using Ollama.Servico.Ollama.Dto;
using Ollama.Servico.Ollama.Interface;
using System.Text;
using System.Text.Json;

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
        private readonly ISessaoMemoriaServico _ISessaoMemoriaServico;
        public OllamaServico(HttpClient httpClient, ISessaoMemoriaServico sessaoMemoriaServico)
        {
            _httpClient = httpClient;
            _ISessaoMemoriaServico = sessaoMemoriaServico;
        }

        public async Task<string> ProcessaEngenhariaPromptDocumentosAsync(string pertunta, string promptMontado, string usuario, CancellationToken cancellationToken)
        {
            var (temperatura, topP) = ObterParametrosTemperatura(EstiloResposta.Rigoroso);
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
                resposta = await EnviarPromptAsync(_servidorLocalUrlBase, body, cts.Token);
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
                await _ISessaoMemoriaServico.RegistrarAsync(new SessaoMemoriaDto
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


        public async Task<string> ProcessaPromptAsync(string promptCompleto, CancellationToken cancellationToken)
        {
            var (temperatura, topP) = ObterParametrosTemperatura(EstiloResposta.Rigoroso);
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
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(_servidorLocalTempoLimiteSegundos));

                return await EnviarPromptAsync(_servidorLocalUrlBase, body, cts.Token);
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


        private async Task<string> EnviarPromptAsync(string appSettingsUrlBase, object requestBody, CancellationToken cancellationToken, bool streaming = false)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{appSettingsUrlBase}/api/generate", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync(cancellationToken);
               // _logger.LogError("Erro Ollama ({Status}): {Erro}", response.StatusCode, erro);
                throw new HttpRequestException(erro);
            }

            var respostaFinal = new StringBuilder();
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var linha = await reader.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(linha)) continue;

                try
                {
                    using var json = JsonDocument.Parse(linha);
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


    
         

        public enum EstiloResposta
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
        private (double temperatura, double topP) ObterParametrosTemperatura(EstiloResposta estilo)
        {
            switch (estilo)
            {
                case EstiloResposta.Rigoroso:
                    return (0.1, 0.3);

                case EstiloResposta.Flexivel:
                    return (0.5, 0.7);

                case EstiloResposta.Criativo:
                    return (0.9, 1.0);

                default:
                    return (0.7, 0.9);
            }
        }


    }
}

//public class EngenhariaPromptDocumentos
//{
//    private static readonly Random _random = new Random();
//    private readonly AppSettingsDto _appSettings;

//    public EngenhariaPromptDocumentos(Microsoft.Extensions.Options.IOptionsMonitor<AppSettingsDto> options)
//    {
//        _appSettings = options.CurrentValue;
//    }


//    public async Task<string> ObterPromptComBaseDocumentos(string pergunta, List<DocumentoContextoDto> documentos, CancellationToken cancellationToken)
//    {
//        // Recupera os trechos de contexto
//        var documentosFiltrados = ObterDocumentosComBaseNaPergunta(pergunta, documentos).ToList();
//        if (documentosFiltrados is null || documentosFiltrados.Count == 0)
//        {
//            return string.Empty;
//        }
//        // Monta o prompt com instruções + contexto relevante
//        var sb = new StringBuilder();
//        sb.AppendLine("Você é um assistente especializado. Use estritamente o contexto abaixo para responder.");
//        sb.AppendLine();
//        sb.AppendLine("--- CONTEXTO RELEVANTE ---");
//        int i = 1;
//        foreach (var d in documentosFiltrados)
//        {
//            sb.AppendLine($"[{i}] {d.Titulo}: {d.Texto}");
//            sb.AppendLine();
//            i++;
//        }
//        sb.AppendLine("--- FIM DO CONTEXTO ---");
//        sb.AppendLine();
//        sb.AppendLine("Pergunta:");
//        sb.AppendLine(pergunta);
//        sb.AppendLine();
//        sb.AppendLine("Instrução: Seja objetivo, indique a fonte [n] quando usar um dos trechos acima. Se não houver informação suficiente, admita que não sabe.");

//        string promptCompleto = sb.ToString();

//        return promptCompleto;
//    }

//    // Busca simples por similaridade: pontua documentos pela frequência de termos (TF simples)
//    private IEnumerable<DocumentoContextoDto> ObterDocumentosComBaseNaPergunta(string pergunta, List<DocumentoContextoDto> documentos)
//    {
//        if (string.IsNullOrWhiteSpace(pergunta) || documentos == null)
//            return Enumerable.Empty<DocumentoContextoDto>();

//        // Quebra o prompt em tokens
//        List<string> tokensPergunta = Tokenizar(pergunta);

//        List<(DocumentoContextoDto Documento, int Score)> documentosComScore = new List<(DocumentoContextoDto, int)>();
//        foreach (var documento in documentos)
//        {
//            var tokensDocumento = Tokenizar($"{documento.Titulo} {documento.Texto}");

//            // Conta quantas vezes cada termo aparece nos tokens
//            int score = 0;
//            foreach (var token in tokensPergunta)
//            {
//                int achou = tokensDocumento.Count(tokenDocumento => tokenDocumento == token);
//                score += achou;
//            }
//            if (score > 0)
//            {
//                documentosComScore.Add((documento, score));
//            }

//        }

//        //Filtra apenas os assuntos relevantes
//        var documentosSelecionados = documentosComScore.Where(x => x.Score > 0).OrderByDescending(x => x.Score).Select(x => x.Documento);

//        return documentosSelecionados;
//    }




//    private static List<string> Tokenizar(string texto)
//    {
//        if (string.IsNullOrWhiteSpace(texto)) return new();

//        texto = texto.ToLowerInvariant();

//        // remove pontuação e divide por espaço
//        var ignorarPalavras = new HashSet<string> {     
//            "a", "o", "os", "as",
//            "um", "uma", "uns", "umas",
//            "de", "do", "da", "dos", "das",
//            "em", "no", "na", "nos", "nas",
//            "para", "por", "com", "sem",
//            "e", "ou", "mas",
//            "que", "se", "sua", "seu", "suas", "seus",
//            "ao", "aos", "à", "às",
//            "sobre", "entre", "até", "após",
//            "como", "quando", "onde",
//            "já", "não", "sim"};
//        var palavras = Regex.Split(texto, @"\W+")     // remove pontuação e divide por espaço
//            .Where(w => w.Length > 1 && !ignorarPalavras.Contains(w))
//            .ToList();

//        return palavras;
//    }
//}
