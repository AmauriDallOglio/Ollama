using Ollama.Servico.Ollama.Dto;
using Ollama.Servico.Ollama.Interface;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Ollama.Servico.Ollama
{
    public class OllamaServico : IOllamaServico
    {
        private readonly HttpClient _httpClient;
        public static readonly string _servidorLocalNome = "Local";
        public static readonly string _servidorLocalUrlBase = "http://localhost:11434";
        public static readonly string _servidorLocalModelo = "llama3.2";
        public static readonly int _servidorLocalTempoLimiteSegundos = 500;
        public static readonly string _servidorLocalIdioma = "pt-BR";
        private readonly ISessaoMemoriaServico _iSessaoMemoriaServico;
        private static readonly Random _random = new Random();

        public OllamaServico(HttpClient httpClient, ISessaoMemoriaServico iSessaoMemoriaServico)
        {
            _httpClient = httpClient;
            _iSessaoMemoriaServico = iSessaoMemoriaServico;
        }

        public async Task<string> ExecutaPromptGeneraticoAsync(string pertunta, string promptMontado, string usuario, CancellationToken cancellationToken)
        {
            var (temperatura, topP) = ObterParametrosTemperatura(EstiloTemperatura.Rigoroso);
            var body = new
            {
                model = _servidorLocalModelo,
                prompt = promptMontado,
                stream = false,
                max_tokens = 512,
                options = new
                {
                    temperature = temperatura,
                    top_p = topP,
                    language = _servidorLocalIdioma,
                }
            };
            string resposta = string.Empty;
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_servidorLocalTempoLimiteSegundos));
                resposta = await ServidorOllama(_servidorLocalUrlBase, body, cts.Token);

                Console.WriteLine("--------------------------------- Resposta ---------------------------------");
                Console.WriteLine(resposta);
                Console.WriteLine();


                return resposta;
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                //_logger.LogError(ex, "Timeout de {TempoLimite}s atingido para o servidor {TipoServidor}", tipoServidorConfig.TempoLimiteSegundos, tipoServidor);
                throw new TimeoutException($"Tempo limite de {_servidorLocalTempoLimiteSegundos}s atingido para {_servidorLocalModelo}");
            }
            catch (TaskCanceledException ex)
            {
               // _logger.LogWarning(ex, "Operação cancelada externamente para o servidor {TipoServidor}", tipoServidor);
                throw;
            }
            finally
            {
                // 5. Registra log da interação para aprendizado supervisionado
                await _iSessaoMemoriaServico.RegistrarAsync(new SessaoMemoriaDto
                {
                    //IdConversa = string.IsNullOrWhiteSpace(idConversa) ? "conversa-padrao" : idConversa.Trim(),
                    Pergunta = pertunta,
                    PromptMontado = promptMontado,
                    RespostaModelo = resposta,
                    Usuario = usuario,
                    RespostaCorreta = false, // Pode ser atualizado via feedback do usuário
                    FeedbackUsuario = string.Empty
                }, cancellationToken);
            }
        }


        public async Task<string> ExecutaPromptAsync(string promptCompleto, CancellationToken cancellationToken)
        {
            var (temperatura, topP) = ObterParametrosTemperatura(EstiloTemperatura.Rigoroso);
            var body = new
            {
                model = _servidorLocalModelo,
                prompt = promptCompleto,
                stream = false,
                max_tokens = 512,
                options = new
                {
                    temperature = temperatura,
                    top_p = topP,
                    language = _servidorLocalIdioma,
                }
            };

            try
            {
                using CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_servidorLocalTempoLimiteSegundos));
                return await ServidorOllama(_servidorLocalUrlBase, body, cts.Token);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                // Timeout atingido
                // _logger.LogError(ex, "Timeout de {TempoLimite}s atingido para o servidor {TipoServidor}", tipoServidorConfig.TempoLimiteSegundos, tipoServidor);
                throw new TimeoutException($"Tempo limite de {_servidorLocalTempoLimiteSegundos}s atingido para {_servidorLocalModelo}");
            }
            catch (TaskCanceledException ex)
            {
                // Cancelamento externo
               // _logger.LogWarning(ex, "Operação cancelada externamente para o servidor {TipoServidor}", tipoServidor);
                throw;
            }
        }


        private async Task<string> ServidorOllama(string appSettingsUrlBase, object requestBody, CancellationToken cancellationToken, bool streaming = false)
        {
            StringContent content = new StringContent(  JsonSerializer.Serialize(requestBody),  Encoding.UTF8, "application/json");
            HttpResponseMessage? response = await _httpClient.PostAsync($"{appSettingsUrlBase}/api/generate", content, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync(cancellationToken);
               // _logger.LogError("Erro Ollama ({Status}): {Erro}", response.StatusCode, erro);
                throw new HttpRequestException(erro);
            }

            StringBuilder respostaFinal = new StringBuilder();
            using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using StreamReader reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                String? linha = await reader.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(linha)) continue;

                try
                {
                    using JsonDocument json = JsonDocument.Parse(linha);
                    if (json.RootElement.TryGetProperty("response", out var parte))
                        respostaFinal.Append(parte.GetString());
                }
                catch (JsonException)
                {
                    // ignora linha inválida
                }
            }
            return respostaFinal.ToString().Trim();
        }




        private enum EstiloTemperatura
        {
            Rigoroso = 0, //Útil para tarefas que exigem precisão, como explicações técnicas, cálculos ou respostas.
            Flexivel = 1, //Bom para quando você quer respostas variadas, mas ainda com certo controle, como brainstorming moderado ou textos explicativos.
            Criativo = 2 //Ideal para tarefas criativas, como histórias, metáforas, ideias fora da caixa ou geração de conteúdo artístico.
        }

        /// <summary>
        /// Temperatura baixa (0.1, 0.8) O modelo gera respostas mais determinísticas, objetivas e previsíveis.
        /// Temperatura intermediária (0.5, 0.9). Equilíbrio entre consistência e criatividade.
        /// Temperatura alta (0.9, 1.0). O modelo gera respostas mais diversas, imaginativas e menos previsíveis.
        /// Top_p baixo (0.2–0.4):  Ideal para respostas técnicas, precisas.  
        /// Top_p médio (0.6–0.8):  Bom equilíbrio entre consistência e diversidade. 
        /// Top_p alto (0.9–1.0):   Util para tarefas criativas, como histórias, brainstorming ou geração de ideias.
        /// </summary>
        /// <param name="estilo"></param>
        /// <returns></returns>
        private (double temperatura, double topP) ObterParametrosTemperatura(EstiloTemperatura estilo)
        {
            switch (estilo)
            {
                case EstiloTemperatura.Rigoroso:
                    return (0.1, 0.3);

                case EstiloTemperatura.Flexivel:
                    return (0.5, 0.7);

                case EstiloTemperatura.Criativo:
                    return (0.9, 1.0);

                default:
                    return (0.7, 0.9);
            }
        }




        public async Task<string> GerarPromptGenerativo(string pergunta, List<DocumentoContextoDto> documentos, CancellationToken cancellationToken)
        {
            List<string> trechosLocalizados = FiltroPalavraChave(pergunta, documentos);
            if (trechosLocalizados.Count == 0)
                return await Task.FromResult(string.Empty);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Você é um assistente especializado em análise de documentos ");
            sb.AppendLine("REGRAS OBRIGATÓRIAS:");
            sb.AppendLine("- NÃO use conhecimento externo.");
            sb.AppendLine("- NÃO faça suposições.");
            sb.AppendLine("- NÃO tente completar informações.");
            sb.AppendLine("- NÃO invente respostas.");
            sb.AppendLine("SE a resposta NÃO estiver claramente no CONTEXTO:");
            sb.AppendLine("RESPONDA EXATAMENTE: Desculpe, não encontrei informações sobre isso na minha base de dados.");

            sb.AppendLine();
            sb.AppendLine($"Pergunta: " + pergunta);
            sb.AppendLine();

            sb.AppendLine("--- CONTEXTO RELEVANTE ---");
            foreach (string trecho in trechosLocalizados)
            {
                sb.AppendLine(trecho);
                sb.AppendLine();
            }
            sb.AppendLine("--- FIM DO CONTEXTO ---");

            Console.WriteLine("------------------------------- Prompt Gerado ------------------------------");
            Console.WriteLine(sb);
 
            //Chama o serviço do Ollama para processar o prompt
            var resultado = await ExecutaPromptGeneraticoAsync(pergunta, sb.ToString(), "Sistema", cancellationToken);
            return resultado;
        }



        private static List<string> FiltroPalavraChave(string pergunta, List<DocumentoContextoDto> documentos)
        {
            List<string> trechosCapturados = new List<string>();

            // Valida entrada
            if (string.IsNullOrWhiteSpace(pergunta) || documentos == null || documentos.Count == 0)
                return trechosCapturados; //Enumerable.Empty<DocumentoContextoDto>();

            Console.WriteLine();
            Console.WriteLine("--------------------------------- Iniciando --------------------------------");
            //Tokenizar a pergunta para obter os tokens de busca
            List<string> listaTokensPergunta = Tokenizar(pergunta);
            if (listaTokensPergunta.Count == 0)
                return trechosCapturados; //Enumerable.Empty<DocumentoContextoDto>();

            // Para cada documento, contar quantos tokens da pergunta aparecem no título e texto
            List<(DocumentoContextoDto Documento, int QuantidadeTokens)> documentosFiltrados = new List<(DocumentoContextoDto, int)>();
            Console.WriteLine("----------------------- Processando documentos --------------------------");
            int qtdpergunta = listaTokensPergunta.Count;
            Console.WriteLine("Quantidade de tokens da frase: " + qtdpergunta);

            foreach (DocumentoContextoDto documento in documentos)
            {
                Console.WriteLine("Processando documento: " + documento.Titulo);

                // Dividir o texto do documento em frases e verificar se alguma frase contém os tokens da pergunta
                string[] frases = documento.Texto.Split(new[] { '.', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                // Verificar se alguma frase contém os tokens da pergunta
                foreach (var frase in frases)
                {
                    string fraseLimpa = Regex.Replace(frase, @"[^a-zA-Z0-9\s]", ""); //remove tudo que não seja letra, número ou espaço
                    int tokensEncontrados = 0;
                    // Verificar se a frase contém algum dos tokens da pergunta, considerando palavras inteiras
                    foreach (var token in listaTokensPergunta)
                    {
                        // Quebra a frase em palavras
                        var palavras = fraseLimpa.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        // Verifica se alguma palavra é exatamente igual ao token (ignorando maiúsculas/minúsculas)
                        if (palavras.Any(p => p.Equals(token, StringComparison.OrdinalIgnoreCase)))
                        {
                            tokensEncontrados += 1; // Incrementa a contagem de tokens encontrados
                        }
                    }

                    if (tokensEncontrados > 0)
                    {
                        Console.WriteLine(frase);
                        var quantidadeTokensFrase = frase.Count();
                        Console.WriteLine("Quantidade de tokens da frase: " + quantidadeTokensFrase);
                        Console.WriteLine("Quantidade de tokens da frase encontrados: " + tokensEncontrados);
                        trechosCapturados.Add(frase.Trim());
                    }
                }
            }
            return trechosCapturados;
        }

        private static List<string> Tokenizar(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return new List<string>();

            string textoNormalizado = texto.ToLowerInvariant();

            HashSet<string> ignorarPalavras = new HashSet<string>
            {
                "a", "o", "os", "as",
                "um", "uma", "uns", "umas",
                "de", "do", "da", "dos", "das",
                "em", "no", "na", "nos", "nas",
                "para", "por", "com", "sem",
                "e", "ou", "mas",
                "que", "se", "sua", "seu", "suas", "seus",
                "ao", "aos", "as", "sobre", "entre", "ate", "apos",
                "como", "quando", "onde",
                "ja", "nao", "sim"
            };

            var takens = Regex.Split(textoNormalizado, @"\W+")
                .Where(w => w.Length > 1 && !ignorarPalavras.Contains(w))
                .Distinct()
                .ToList();

            // Concatenar todos os tokens separados por espaço
            string resultado = string.Join(" ", takens);
            Console.WriteLine(resultado);

            return takens;
        }





        private string PromptOrdemServico(string manutentor)
        {
            string persona = @$"
                Você é um especialista em PCM (Planejamento e Controle da Manutenção) e organizador do trabalho dos manutentores. 
                Sua função é analisar a lista de Ordens de Serviço recebida, avaliar os prazos e status, e organizar as atividades como se fosse o chefe da manutenção orientando o manutentor {manutentor}. 
                Sempre fale de forma objetiva e clara, simulando uma comunicação prática de rotina como um técnico de manutenção.
            ";

            string contexto = @$"
                - Inicialize falando: Olá {manutentor} bom dia, seu cronograma de trabalho para hoje: .
                - Analise a lista de Ordens de Serviço recebida em texto, destinada para o manutentor {manutentor} .
                - Manutentor {manutentor} tem disponibilidade total de trabalho de 8h por dia, iniciando seu turno as 05:00 até as 13:30, parando das 09:00 até as 09:30 para descanso.
                - Calculando meu inicio de trabalho na hora de execução desse prompt.
                - Informe de forma clara:
                    1. Quantas ordens de serviço estão atrasadas, apresentando abaixo os registros em tabela.
                    2. Quantas podem ser atendidas no dia de hoje {DateTime.Now}, apresentando abaixo os registros em tabela.
                    3. Quantas podem ser atendidas no futuro, apresentando abaixo os registros em tabela.
                - Priorize as ordens com status 'Parada', 'EmExecucao', 'Agendada'. 
                - Gere também uma estimativa do que o manutentor deve priorizar hoje, como se fosse uma orientação direta do chefe da manutenção.
                - Apresente os dados de forma organizada em lista ou tópicos.
                - Finalize com um resumo prático: 'Bom trabalho!'.
                - Apresente a lista completa das ordens de serviço recebida, organizada por data de cadastro, do mais antigo para o mais recente.
            ";

            string ordensServico = ObterListaOrdemServico(manutentor);
            return MontarPrompt(persona, contexto, ordensServico);
        }

        public string PromptOrdemServicoHtml(string manutentor)
        {
            string persona = @$"
                Você é um especialista em PCM (Planejamento e Controle da Manutenção) e organizador do trabalho dos manutentores. 
                Sua função é analisar a lista de Ordens de Serviço recebida, avaliar os prazos e status, e organizar as atividades como se fosse o chefe da manutenção orientando o manutentor {manutentor}. 
                Sempre fale de forma objetiva e clara, simulando uma comunicação prática de rotina como um técnico de manutenção.
            ";

            string dataatual = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            string contexto = @$"
                - Inicialize falando: Olá {manutentor}, bom dia! Seu cronograma de trabalho para hoje: .
                - Analise a lista de Ordens de Serviço recebida em texto, destinada para o manutentor {manutentor} .
                - Manutentor {manutentor} tem disponibilidade total de trabalho de 8h por dia, iniciando seu turno as 08:00 até as 18:00, parando das 12:00 até as 13:30 para descanso.
                - Calculando meu inicio de trabalho as {dataatual}.
                - Priorize as ordens com status 'Parada', 'EmExecucao', 'Agendada'. 
                - Gere também uma estimativa do que o manutentor deve priorizar hoje, como se fosse uma orientação direta do chefe da manutenção.
                - Finalize com um resumo prático: 'Bom trabalho!'.
                - Gere a resposta em html e css para ser apresentada em um navegador.
            ";

            string ordensServico = ObterListaOrdemServico(manutentor);
            return MontarPrompt(persona, contexto, ordensServico);
        }

        private string ObterListaOrdemServico(string manutentor)
        {
            List<OrdemServicoDto>? listaOrdensServico = GerarListaOrdensServico(manutentor);
            string ordens = OrdemServicoConverterParaTexto(listaOrdensServico);
            return ordens;
        }

        private string MontarPrompt(string persona, string contexto, string prompt)
        {

            PromptResponseDto promptDto = new PromptResponseDto(persona, contexto, prompt);
            string promptFormatado = string.Join("\n", promptDto.Mensagens.Select(m => $"{m.Papel.ToUpper()}: {m.Conteudo}"));

            return promptFormatado;
        }

        private string OrdemServicoConverterParaTexto(List<OrdemServicoDto> ordens)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Lista de Ordens de Serviço:");
            foreach (var os in ordens)
            {
                sb.AppendLine($"- Código: {os.Codigo}, Data: {os.DataCadastro:dd/MM/yyyy}, " +
                              $"Status: {os.Status}, Tempo Estimado: {os.TempoEstimadoHoras}h, " +
                              $"Manutentor: {os.Manutentor}");
            }
            return sb.ToString();
        }


        private List<OrdemServicoDto> GerarListaOrdensServico(string nomeManutentor)
        {
            var ordens = new List<OrdemServicoDto>();
            DateTime hoje = DateTime.Now;
            int codigo = 1;

            // 10 passadas
            for (int i = 1; i <= 8; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje.AddDays(-i), nomeManutentor, "Maximus Decimus Meridius"));
            }

            // 5 no dia atual
            for (int i = 0; i < 12; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje, nomeManutentor, "Maximus Decimus Meridius"));
            }

            // 15 futuras
            for (int i = 1; i <= 15; i++)
            {
                ordens.Add(CriarOrdem(codigo++, hoje.AddDays(i), nomeManutentor, "Maximus Decimus Meridius"));
            }

            return ordens;
        }

        private static OrdemServicoDto CriarOrdem(int codigo, DateTime data, string manutentor1, string manutentor2)
        {
            return new OrdemServicoDto
            {
                Codigo = codigo.ToString(),
                DataCadastro = data,
                Status = GerarStatusAleatorio(),
                TempoEstimadoHoras = (int)Math.Round(_random.NextDouble() * 8 + 1, 2), // entre 1 e 9 horas
                Manutentor = codigo % 2 == 0 ? manutentor1 : manutentor2
            };
        }

        private static StatusOrdemServico GerarStatusAleatorio()
        {
            var valores = Enum.GetValues(typeof(StatusOrdemServico));
            return (StatusOrdemServico)valores.GetValue(_random.Next(valores.Length))!;
        }


    }
}