using Microsoft.Extensions.Configuration;
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
        private readonly OllamaAppSettingsDto _OllamaAppSettingsDto_Local;
        private readonly OllamaAppSettingsDto _OllamaAppSettingsDto_Docker;
        private readonly ILogger<OllamaServico> _logger;


        private readonly IConfiguration _config;
        private readonly PromptDocumentoServico _contexto;

        public OllamaServico(HttpClient httpClient, IConfiguration config, PromptDocumentoServico contexto, IOptions<OllamaAppSettingsDto> ollamaAppSettingsDto, ILogger<OllamaServico> logger, IOptionsMonitor<OllamaAppSettingsDto> options)
        {
            _httpClient = httpClient;
 

            _OllamaAppSettingsDto_Local = options.Get("Local");
            _OllamaAppSettingsDto_Docker = options.Get("Docker");


            _logger = logger;


            _config = config;
            _contexto = contexto;
        }



        public Task<string> ProcessaPromptLocalAsync(string prompt, CancellationToken ct)
        {
            var body = new 
            { 
                model = _OllamaAppSettingsDto_Local.Modelo,
                prompt, 
                stream = false 
            };

            return EnviarPromptAsync(_OllamaAppSettingsDto_Local, body, ct);
        }

        public async Task<string> ProcessaPromptLocalContextoAsync(string promptCompleto, CancellationToken ct)
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(_OllamaAppSettingsDto_Local.TempoLimite));

            var body = new
            {
                model = _OllamaAppSettingsDto_Local.Modelo,
                prompt = promptCompleto,
                stream = false,
                max_tokens = 512,
                temperature = 0.0
            };

            return await EnviarPromptAsync(_OllamaAppSettingsDto_Local, body, cts.Token);
        }

        public Task<string> ProcessaPromptDockerAsync(string prompt, CancellationToken ct)
        {
            var body = new 
            { 
                model = _OllamaAppSettingsDto_Docker.Modelo, 
                prompt, 
                stream = true 
            };

            return EnviarPromptAsync(_OllamaAppSettingsDto_Docker, body, ct, streaming: true);
        }


        private async Task<string> EnviarPromptAsync(  OllamaAppSettingsDto settings,  object requestBody, CancellationToken cancellationToken, bool streaming = false)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{settings.BaseUrl}/api/generate", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Erro Ollama ({Status}): {Erro}", response.StatusCode, erro);
                throw new HttpRequestException(erro);
            }

            //if (!streaming)
            //{
            //    var json = await response.Content.ReadAsStringAsync(cancellationToken);
            //    return ExtrairResposta(json);
            //}

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





        //public async Task<string> ProcessaPromptAsync(string prompt, CancellationToken cancellationToken)
        //{
        //    var requestBody = new
        //    {
        //        model = _OllamaAppSettingsDto_Local.Modelo,
        //        prompt = prompt,
        //        stream = false
        //    };

        //    var content = new StringContent(
        //        JsonSerializer.Serialize(requestBody),
        //        Encoding.UTF8,
        //        "application/json");

        //    HttpResponseMessage response = await _httpClient.PostAsync($"{_OllamaAppSettingsDto_Local.BaseUrl}/api/generate", content, cancellationToken);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var erro = await response.Content.ReadAsStringAsync(cancellationToken);
        //        _logger.LogError("Erro ao comunicar com Ollama: {Erro}", erro);
        //        return $"Erro ao comunicar com o Ollama: {erro}";
        //    }

        //    var json = await response.Content.ReadAsStringAsync(cancellationToken);

        //    using var doc = JsonDocument.Parse(json);
        //    return doc.RootElement.TryGetProperty("response", out var resposta)
        //        ? resposta.GetString() ?? string.Empty
        //        : json;
        //}


        //public async Task<string> ProcessaPromptContextoAsync(string promptCompleto, CancellationToken cancellationToken)
        //{
        //    //var stopwatch = Stopwatch.StartNew();

        //    // Prepara corpo para Ollama(ex.: / api / generate)
        //    var requestBody = new
        //    {
        //        model = _OllamaAppSettingsDto_Local.Modelo,
        //        prompt = promptCompleto,
        //        stream = false,
        //        max_tokens = 512,
        //        temperature = 0.0,
        //        // stream = false
        //    };
        //    var content = new StringContent(
        //        JsonSerializer.Serialize(requestBody),
        //        Encoding.UTF8,
        //        "application/json");

        //    HttpResponseMessage response = await _httpClient.PostAsync($"{_OllamaAppSettingsDto_Local.BaseUrl}/api/generate", content, cancellationToken);

        //    if (!response.IsSuccessStatusCode)
        //    {
        //        var erro = await response.Content.ReadAsStringAsync(cancellationToken);
        //        _logger.LogError("Erro ao comunicar com Ollama: {Erro}", erro);
        //        return $"Erro ao comunicar com o Ollama: {erro}";
        //    }

        //    // Chamada HTTP
        //    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        //    int timeoutSeconds = int.TryParse(_config["Ollama:TimeoutSeconds"], out var t) ? t : 30;
        //    cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

        //    // Parse da resposta (ajuste conforme formato retornado por sua versão do Ollama)
        //    var json = await response.Content.ReadAsStringAsync(cts.Token);




        //    using var doc = JsonDocument.Parse(json);
        //    return doc.RootElement.TryGetProperty("response", out var resposta)
        //        ? resposta.GetString() ?? string.Empty
        //        : json;

        //    //stopwatch.Stop();
        //    // return new OllamaResponseDto(textoResposta, stopwatch.ElapsedMilliseconds);
        //}

        //public async Task<string> PerguntarDockerAsync(string prompt, CancellationToken cancellationToken)
        //{
        //    var respostaFinal = new StringBuilder();
        //    try
        //    {


        //        using var _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(_OllamaAppSettingsDto_Docker.TempoLimite) };

        //        var payload = new
        //        {
        //            model = _OllamaAppSettingsDto_Docker.Modelo,
        //           prompt = prompt, 
        //            stream = false
        //        };

        //        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        //        var response = await _httpClient.PostAsync($"{_OllamaAppSettingsDto_Local.BaseUrl}/api/generate", content, cancellationToken);


        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var erro = await response.Content.ReadAsStringAsync(cancellationToken);
        //            _logger.LogError("Erro ao comunicar com Ollama: {Erro}", erro);
        //            return $"Erro ao comunicar com o Ollama: {erro}";

        //        }

        //        using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        //        using var reader = new StreamReader(stream);

        //        while (!reader.EndOfStream)
        //        {
        //            var linha = await reader.ReadLineAsync(cancellationToken);
        //            if (string.IsNullOrWhiteSpace(linha)) continue;

        //            var json = JsonDocument.Parse(linha);
        //            if (json.RootElement.TryGetProperty("response", out var parte))
        //            {
        //                respostaFinal.Append(parte.GetString());
        //            }
        //        }

        //        return respostaFinal.ToString().Trim();
        //    }
        //    catch (TaskCanceledException ex)
        //    {
        //        _logger.LogError("Tempo de resposta excedido. A operação foi cancelada após o tempo limite configurado.");
        //        return $"Tempo de resposta excedido. A operação foi cancelada após o tempo limite configurado. {ex.Message}";
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Erro inesperado.");
        //        return $"Erro inesperado.  {ex.Message}";
        //    }
        //}
    }
}
