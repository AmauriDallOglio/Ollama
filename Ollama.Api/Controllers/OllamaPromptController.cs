using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using System.Diagnostics;
using static Ollama.Aplicacao.Servico.ContextoServico;

namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaPromptController : ControllerBase
    {
        private readonly ILogger<OllamaPromptController> _logger;
        private readonly OllamaServico _OllamaServico;
        private readonly HelperConsoleColor _helper;

        public OllamaPromptController(OllamaServico ollamaServico, ILogger<OllamaPromptController> logger, HelperConsoleColor helper)
        {
            _OllamaServico = ollamaServico;
            _logger = logger;
            _helper = helper;   
        }



        [HttpPost("PerguntaEmGeral")]
        public async Task<IActionResult> PerguntaEmGeral([FromBody] PerguntaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Assunto))
                return BadRequest("A pergunta não pode ser vazia.");

            try
            {
                var resposta = await _OllamaServico.PerguntarAsync(request.Assunto);
          
                // return Ok(new { resposta = response.Text() });
                return Content(resposta, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na chamada da API Gemini: {ex.Message}");
                return StatusCode(500, new { error = "Ocorreu um erro ao se comunicar com a API Gemini.", details = ex.Message });
            }
        }



       
    }
}
