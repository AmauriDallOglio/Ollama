using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using System.Diagnostics;

namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaController : ControllerBase
    {
        private readonly ILogger<OllamaController> _logger;
        private readonly OllamaServico _OllamaServico;
        private readonly HelperConsoleColor _helper;

        public OllamaController(OllamaServico ollamaServico, ILogger<OllamaController> logger, HelperConsoleColor helper)
        {
            _OllamaServico = ollamaServico;
            _logger = logger;
            _helper = helper;   
        }

        [HttpGet("pergunta")]
        public async Task<IActionResult> Perguntar([FromQuery] string texto)
        {
            var total = Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(texto))
            {
                string mensagem = "Informe uma pergunta válida.";

                _helper.Erro($"{mensagem}");
                return BadRequest(new { erro = mensagem });
            }
       

            var resposta = await _OllamaServico.PerguntarAsync(texto);

            total.Stop();
 

            var objeto = new
            {
                pergunta = texto,
                resposta,
                tempoTotal = $"{total.ElapsedMilliseconds} ms"
            };


            _helper.Informacao($"{objeto}");
            return Ok(objeto);
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
