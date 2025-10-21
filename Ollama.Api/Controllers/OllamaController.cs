using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Servico;

namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaController : ControllerBase
    {

        private readonly OllamaServico _OllamaServico;

        public OllamaController(OllamaServico ollamaServico)
        {
            _OllamaServico = ollamaServico;
        }

        [HttpGet("pergunta")]
        public async Task<IActionResult> Perguntar([FromQuery] string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return BadRequest(new { erro = "Informe uma pergunta válida." });

            var resposta = await _OllamaServico.PerguntarAsync(texto);
            return Ok(new
            {
                pergunta = texto,
                resposta
            });
        }


        //private readonly HttpClient _httpClient;

        //public OllamaController(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClient = httpClientFactory.CreateClient();
        //}

        //[HttpPost("PerguntaEmGeral")]
        //public async Task<IActionResult> PerguntaEmGeral([FromBody] PerguntaRequest request)
        //{
        //    if (request == null || string.IsNullOrWhiteSpace(request.Pergunta))
        //        return BadRequest("A pergunta não pode ser vazia.");

        //    try
        //    {
        //        var ollamaRequest = new
        //        {
        //            model = "llama3.2", // modelo que você baixou
        //            prompt = request.Pergunta
        //        };

        //        var response = await _httpClient.PostAsJsonAsync("http://localhost:11434/api/generate", ollamaRequest);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            var error = await response.Content.ReadAsStringAsync();
        //            return StatusCode((int)response.StatusCode, new { error = "Falha ao se comunicar com o Ollama.", detalhes = error });
        //        }

        //        // Lê toda a resposta (vários JSONs por linha)
        //        var respostaBruta = await response.Content.ReadAsStringAsync();

        //        // Divide em linhas e pega apenas o campo "response"
        //        var linhas = respostaBruta.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        //        var respostaFinal = "";

        //        foreach (var linha in linhas)
        //        {
        //            try
        //            {
        //                using var doc = JsonDocument.Parse(linha);
        //                if (doc.RootElement.TryGetProperty("response", out var resp))
        //                    respostaFinal += resp.GetString();
        //            }
        //            catch
        //            {
        //                // ignora linhas inválidas
        //            }
        //        }

        //        return Ok(new
        //        {
        //            pergunta = request.Pergunta,
        //            resposta = respostaFinal.Trim()
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erro ao se comunicar com o Ollama: {ex.Message}");
        //        return StatusCode(500, new { error = "Erro interno ao processar a requisição.", details = ex.Message });
        //    }
        //}
    }

    //public class PerguntaRequest
    //{
    //    public string Pergunta { get; set; } = string.Empty;
    //}

}
