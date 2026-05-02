using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Rotas.OllamaRota;
using Ollama.Servico.Ollama.Interface;

namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly PromptHandler _PromptHandler;
        private readonly PromptGenerativoHandler _PromptGenerativoHandler;
        private readonly ISessaoMemoriaServico _ISessaoMemoriaServico;
        private readonly PromptGenerativoDadosMocadosHandler _PromptGenerativoDadosMocadosHandler;
        public OllamaController(
            ISessaoMemoriaServico iSessaoMemoriaServico,
            IWebHostEnvironment env,
            PromptHandler promptHandler,
            PromptGenerativoHandler promptGenerativoHandler,
            PromptGenerativoDadosMocadosHandler promptGenerativoDadosMocadosHandler
            )
        {
            _ISessaoMemoriaServico = iSessaoMemoriaServico;
            _env = env;
            _PromptHandler = promptHandler;
            _PromptGenerativoHandler = promptGenerativoHandler;
            _PromptGenerativoDadosMocadosHandler = promptGenerativoDadosMocadosHandler;
        }


        [HttpGet("Prompt")]
        public async Task<IActionResult?> Prompt([FromQuery] PromptRequest request, CancellationToken cancellationToken)
        {
            var resultado = await _PromptHandler.Executar(request, cancellationToken);

            if (resultado.Sucesso)
                return Ok(resultado.Resultado);
            else
                return BadRequest(resultado.Mensagem);
        }

        [HttpGet("PromptGenerativo")]
        public async Task<IActionResult> PromptGenerativo([FromQuery] PromptGenerativoRequest request, CancellationToken cancellationToken)
        {
            var resultado = await _PromptGenerativoHandler.Executar(request, cancellationToken);

            if (resultado.Sucesso)
                return Ok(resultado.Resultado);
            else
                return BadRequest(resultado);
        }


        [HttpGet("ObterMemoria")]
        public async Task<IActionResult> ObterMemoria(CancellationToken cancellationToken)
        {
            var logs = await _ISessaoMemoriaServico.ObterTodosAsync(cancellationToken);
            return Ok(logs);
        }


        [HttpGet("PromptGenerativoDadosMocados")]
        public async Task<IActionResult> PromptGenerativoDadosMocados([FromQuery] PromptGenerativoDadosMocadosRequest request, CancellationToken cancellationToken)
        {
            //var request = new PromptGenerativoDadosMocadosRequest { Pergunta = "Amauri" };
            var resultado = await _PromptGenerativoDadosMocadosHandler.Executar(request, cancellationToken);

            if (resultado.Sucesso)
                return Ok(resultado.Resultado);
            else
                return BadRequest(resultado.Mensagem);
        }



    }
}
