using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ollama.Aplicacao.Dto;
using System.Text;
using System.Text.Json;

namespace Ollama.Aplicacao.Servico
{
    public class OllamaServico
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettingsDto _appSettings;
        private readonly ILogger<OllamaServico> _logger;

        public OllamaServico(HttpClient httpClient, ILogger<OllamaServico> logger, IOptionsMonitor<AppSettingsDto> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _appSettings = options.CurrentValue;
        }

        public async Task<string> ProcessaPromptAsync(string promptCompleto, CancellationToken cancellationToken)
        {


            int tipoServidor = _appSettings.TipoServidor.Tipo;
            var tipoServidorConfig = ObterServidorInfo((TipoServidor)tipoServidor);

            if (string.IsNullOrEmpty(tipoServidorConfig.UrlBase))
            {
                _logger.LogError("Configuração inválida para o tipo de servidor: {TipoServidor}", tipoServidor);
                throw new InvalidOperationException($"Configuração inválida para o tipo de servidor: {tipoServidor}");
            }

            var (temperatura, topP) = ObterParametrosTemperatura(EstiloResposta.Rigoroso);

            var body = new
            {
                model = tipoServidorConfig.Modelo,
                prompt = promptCompleto,
                stream = false,
                max_tokens = 512,
                options = new
                {
                    temperature = temperatura,
                    top_p = topP,
                    language = tipoServidorConfig.Idioma,
                }
            };

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(tipoServidorConfig.TempoLimiteSegundos));

                return await EnviarPromptAsync(tipoServidorConfig.UrlBase, body, cts.Token);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                // Timeout atingido
                _logger.LogError(ex, "Timeout de {TempoLimite}s atingido para o servidor {TipoServidor}", tipoServidorConfig.TempoLimiteSegundos, tipoServidor);
                throw new TimeoutException($"Tempo limite de {tipoServidorConfig.TempoLimiteSegundos}s atingido para {tipoServidor}");
            }
            catch (TaskCanceledException ex)
            {
                // Cancelamento externo
                _logger.LogWarning(ex, "Operação cancelada externamente para o servidor {TipoServidor}", tipoServidor);
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
                _logger.LogError("Erro Ollama ({Status}): {Erro}", response.StatusCode, erro);
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


        private TipoServidorSelecionado ObterServidorInfo(TipoServidor tipoServidor)
        {
            switch (tipoServidor)
            {
                case TipoServidor.ServidorLocal:
                    return new TipoServidorSelecionado().CarregarLocal(_appSettings);

                case TipoServidor.ServidorDocker:
                    return new TipoServidorSelecionado().CarregarDocker(_appSettings);

                default:
                    return new TipoServidorSelecionado();
            }
        }

        public enum TipoServidor
        {
            ServidorLocal = 0,
            ServidorDocker = 1
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
