using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ollama.Aplicacao.Dto;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Ollama.Aplicacao.Servico
{
    public class OllamaServico
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaAppSettingsDto _OllamaAppSettingsDto;
        private readonly ILogger<OllamaServico> _logger;


        private readonly IConfiguration _config;
        private readonly PromptDocumentoServico _contexto;

        public OllamaServico(HttpClient httpClient, IConfiguration config, PromptDocumentoServico contexto, IOptions<OllamaAppSettingsDto> ollamaAppSettingsDto, ILogger<OllamaServico> logger)
        {
            _httpClient = httpClient;
            _OllamaAppSettingsDto = ollamaAppSettingsDto.Value;
            _logger = logger;


            _config = config;
            _contexto = contexto;
        }

        public async Task<string> ProcessaPromptAsync(string prompt, CancellationToken cancellationToken)
        {
            var requestBody = new
            {
                model = _OllamaAppSettingsDto.Modelo,
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_OllamaAppSettingsDto.BaseUrl}/api/generate", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Erro ao comunicar com Ollama: {Erro}", erro);
                return $"Erro ao comunicar com o Ollama: {erro}";
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("response", out var resposta)
                ? resposta.GetString() ?? string.Empty
                : json;
        }


        public async Task<string> ProcessaPromptContextoAsync(string promptCompleto, CancellationToken cancellationToken)
        {
            //var stopwatch = Stopwatch.StartNew();

            // Prepara corpo para Ollama(ex.: / api / generate)
            var requestBody = new
            {
                model = _OllamaAppSettingsDto.Modelo,
                prompt = promptCompleto,
                stream = false,
                max_tokens = 512,
                temperature = 0.0,
                // stream = false
            };
            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{_OllamaAppSettingsDto.BaseUrl}/api/generate", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Erro ao comunicar com Ollama: {Erro}", erro);
                return $"Erro ao comunicar com o Ollama: {erro}";
            }

            // Chamada HTTP
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            int timeoutSeconds = int.TryParse(_config["Ollama:TimeoutSeconds"], out var t) ? t : 30;
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            // Parse da resposta (ajuste conforme formato retornado por sua versão do Ollama)
            var json = await response.Content.ReadAsStringAsync(cts.Token);

 

   
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("response", out var resposta)
                ? resposta.GetString() ?? string.Empty
                : json;

            //stopwatch.Stop();
            // return new OllamaResponseDto(textoResposta, stopwatch.ElapsedMilliseconds);
        }

        public async Task<string> PerguntarDockerAsync(string prompt, CancellationToken cancellationToken)
        {
            var respostaFinal = new StringBuilder();
            try
            {
                using var _httpClient = new HttpClient
                {
                    Timeout = TimeSpan.FromMinutes(10)
                };
                var payload = new
                {
                    model = "ai/llama3.2",
                    prompt = prompt,
                    stream = true
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(
                    "http://localhost:11434/api/generate",
                    content,
                    cancellationToken
                );

                if (!response.IsSuccessStatusCode)
                {
                    var erro = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Erro ao comunicar com Ollama: {Erro}", erro);
                    return $"Erro ao comunicar com o Ollama: {erro}";

                }

                using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                using var reader = new StreamReader(stream);

                while (!reader.EndOfStream)
                {
                    var linha = await reader.ReadLineAsync(cancellationToken);
                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    var json = JsonDocument.Parse(linha);
                    if (json.RootElement.TryGetProperty("response", out var parte))
                    {
                        respostaFinal.Append(parte.GetString());
                    }
                }

                return respostaFinal.ToString().Trim();
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError("Tempo de resposta excedido. A operação foi cancelada após o tempo limite configurado.");
                return $"Tempo de resposta excedido. A operação foi cancelada após o tempo limite configurado. {ex.Message}";
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro inesperado.");
                return $"Erro inesperado.  {ex.Message}";
            }
        }
    }
}
