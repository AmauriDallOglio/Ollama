using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using System.Diagnostics;

namespace Ollama.Api.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OllamaDockerController : ControllerBase
    {
        private readonly ILogger<OllamaPromptController> _logger;
        private readonly OllamaServico _OllamaServico;
        private readonly HelperConsoleColor _helper;
        private readonly PromptDocumentoServico _contextoServico;

        public OllamaDockerController(OllamaServico ollamaServico, PromptDocumentoServico contextoServico, ILogger<OllamaPromptController> logger, HelperConsoleColor helper)
        {
            _OllamaServico = ollamaServico;
            _logger = logger;
            _helper = helper;
            _contextoServico = contextoServico;
        }

        [HttpGet("PerguntaDocker")]
        public async Task<IActionResult> PerguntaDocker([FromQuery] string pergunta, CancellationToken cancellationToken)
        {
            try
            {
                var tempo = Stopwatch.StartNew();
                if (string.IsNullOrWhiteSpace(pergunta))
                    return BadRequest(new { erro = "Informe uma pergunta válida." });


                var resposta = await _OllamaServico.ProcessaPromptDockerAsync(pergunta, cancellationToken);
                tempo.Stop();
                var objeto = new
                {
                    pergunta = pergunta,
                    resposta,
                    Tempo = $"{tempo.ElapsedMilliseconds} ms"
                };

                _helper.Informacao($"{objeto}");
                return Ok(objeto);
            }
            catch (Exception)
            {

                throw;
            }
   
        }

    }
}
