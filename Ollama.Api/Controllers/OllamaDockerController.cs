using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
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
            OllamaResponseDto ollamaResponseDto = new();
            var tempo = Stopwatch.StartNew();
            try
            {
                if (string.IsNullOrWhiteSpace(pergunta))
                {
                    tempo.Stop();
                    ollamaResponseDto = new OllamaResponseDto().GeraErro(pergunta, "Informe uma pergunta válida.", tempo.ElapsedMilliseconds);
                    _helper.Informacao($"{ollamaResponseDto}");
                    return BadRequest(ollamaResponseDto);
                }

                var resposta = await _OllamaServico.ProcessaPromptLocalContextoAsync(pergunta, OllamaServico.TipoServidor.ServidorLocal, cancellationToken);
                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraSucesso(pergunta, resposta, tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return Ok(ollamaResponseDto);
            }
            catch (Exception ex)
            {
                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraErro(pergunta, ex.Message, tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return BadRequest(ollamaResponseDto);
            }
   
        }

    }
}
