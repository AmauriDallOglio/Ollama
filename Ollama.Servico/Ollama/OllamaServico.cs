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
        private readonly ISessaoMemoriaServico _iSessaoMemoriaServico;
        public OllamaServico(HttpClient httpClient, ISessaoMemoriaServico iSessaoMemoriaServico)
        {
            _httpClient = httpClient;
            _iSessaoMemoriaServico = iSessaoMemoriaServico;
        }

        public async Task<string> ProcessaPromptDocumentosAsync(string pertunta, string promptMontado, string usuario, CancellationToken cancellationToken)
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
                resposta = await ServidorOllama(_servidorLocalUrlBase, body, cts.Token);
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