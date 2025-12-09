using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ollama.Aplicacao.Dto;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using static Ollama.Aplicacao.Servico.ContextoServico;

namespace Ollama.Aplicacao.Servico
{
    public class OllamaServico
    {
        private readonly HttpClient _httpClient;
        private readonly OllamaAppSettingsDto _OllamaAppSettingsDto;
        private readonly ILogger<OllamaServico> _logger;


        private readonly IConfiguration _config;
        private readonly ContextoServico _contexto;

        public OllamaServico(HttpClient httpClient, IConfiguration config, ContextoServico contexto, IOptions<OllamaAppSettingsDto> ollamaAppSettingsDto, ILogger<OllamaServico> logger)
        {
            _httpClient = httpClient;
            _OllamaAppSettingsDto = ollamaAppSettingsDto.Value;
            _logger = logger;


            _config = config;
            _contexto = contexto;
        }

        public async Task<string> PerguntarAsync(string prompt)
        {
            var body = new
            {
                model = _OllamaAppSettingsDto.Modelo,
                prompt = prompt,
                stream = false
            };

            var content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_OllamaAppSettingsDto.BaseUrl}/api/generate", content);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erro ao comunicar com Ollama: {Erro}", erro);
                return $"Erro ao comunicar com o Ollama: {erro}";
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("response", out var resposta)
                ? resposta.GetString() ?? string.Empty
                : json;
        }


        public async Task<OllamaResponseDto> PerguntarComContextoAsync(string assunto, int topK = 3, CancellationToken ct = default)
        {
            var stopwatch = Stopwatch.StartNew();

            // 1) Recupera os trechos de contexto
            var docs = _contexto.BuscarDocumentos(assunto, topK).ToList();

            // 2) Monta o prompt com instruções + contexto relevante
            var sb = new StringBuilder();
            sb.AppendLine("Você é um assistente especializado. Use estritamente o contexto abaixo para responder.");
            sb.AppendLine();
            sb.AppendLine("--- CONTEXTO RELEVANTE ---");
            int i = 1;
            foreach (var d in docs)
            {
                sb.AppendLine($"[{i}] {d.Titulo}: {d.Texto}");
                sb.AppendLine();
                i++;
            }
            sb.AppendLine("--- FIM DO CONTEXTO ---");
            sb.AppendLine();
            sb.AppendLine("Pergunta:");
            sb.AppendLine(assunto);
            sb.AppendLine();
            sb.AppendLine("Instrução: Seja objetivo, indique a fonte [n] quando usar um dos trechos acima. Se não houver informação suficiente, admita que não sabe.");

            var promptCompleto = sb.ToString();

            //3) Prepara corpo para Ollama(ex.: / api / generate)
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

            HttpResponseMessage response = await _httpClient.PostAsync($"{_OllamaAppSettingsDto.BaseUrl}/api/generate", content);

            // 4) Chamada HTTP
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            int timeoutSeconds = int.TryParse(_config["Ollama:TimeoutSeconds"], out var t) ? t : 30;
            cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            // 5) Parse da resposta (ajuste conforme formato retornado por sua versão do Ollama)
            var json = await response.Content.ReadAsStringAsync(cts.Token);

            // Exemplo simples: assume que a resposta tem um campo 'response' ou 'text'
            string textoResposta;
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("response", out var respProp))
                    textoResposta = respProp.GetString() ?? string.Empty;
                else if (doc.RootElement.TryGetProperty("text", out var textProp))
                    textoResposta = textProp.GetString() ?? string.Empty;
                else
                    textoResposta = json;
            }
            catch
            {
                textoResposta = json;
            }

            stopwatch.Stop();
            return new OllamaResponseDto(textoResposta, stopwatch.ElapsedMilliseconds);
        }
    }
}
