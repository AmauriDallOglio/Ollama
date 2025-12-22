using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;

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
        public async Task<IActionResult> PerguntaEmGeral([FromQuery] string pergunta, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(pergunta))
                return BadRequest("A pergunta não pode ser vazia.");

            try
            {
                string resposta = await _OllamaServico.ProcessaPromptLocalAsync(pergunta, cancellationToken);
          
                // return Ok(new { resposta = response.Text() });
                return Content(resposta, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na chamada da API Gemini: {ex.Message}");
                return StatusCode(500, new { error = "Ocorreu um erro ao se comunicar com a API Gemini.", details = ex.Message });
            }
        }


        [HttpPost("EspecialistaOrdemServico")]
        public async Task<IActionResult> EspecialistaOrdemServico([FromQuery] string manutentor, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(manutentor))
                return BadRequest(new { erro = "O nome do manutentor não pode ser vazio." });

          
            PromptResponseDto promptDto = new EngenhariaPromptServico().PromptOrdemServico(manutentor);

            string texto = promptDto.FormataToString();
            string resultado = await _OllamaServico.ProcessaPromptLocalAsync(texto, cancellationToken);

         

            //var json Ok(new
            //{
            //    Pergunta = request.Pergunta,
            //    Resposta = response.Text
            //});

            return Content(resultado, "application/json");

        }


        [HttpPost("EspecialistaOrdemServicoHtml")]
        public async Task<IActionResult> EspecialistaOrdemServicoHtml([FromQuery] string manutentor, CancellationToken cancellationToken)
        {
            if ( string.IsNullOrWhiteSpace(manutentor))
                return BadRequest(new { erro = "O nome do manutentor não pode ser vazio." });

    
            PromptResponseDto promptDto = new EngenhariaPromptServico().PromptOrdemServicoHtml(manutentor);

            string texto = promptDto.FormataToString();
            string resultado = await _OllamaServico.ProcessaPromptLocalContextoAsync(texto, cancellationToken);

            return Content(resultado, "application/json");
        }

    }
}
