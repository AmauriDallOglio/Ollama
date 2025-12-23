using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using System.Diagnostics;

namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaContextoController : ControllerBase
    {
        private readonly ILogger<OllamaPromptController> _logger;
        private readonly OllamaServico _OllamaServico;
        private readonly HelperConsoleColor _helper;
        private readonly PromptDocumentoServico _contextoServico;

        public OllamaContextoController(OllamaServico ollamaServico, PromptDocumentoServico contextoServico, ILogger<OllamaPromptController> logger, HelperConsoleColor helper)
        {
            _OllamaServico = ollamaServico;
            _logger = logger;
            _helper = helper;
            _contextoServico = contextoServico;
        }

        [HttpGet("PerguntaEmGeral")]
        public async Task<IActionResult> PerguntaEmGeral([FromQuery] string pergunta, CancellationToken cancellationToken)
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

                var resposta = await _OllamaServico.ProcessaPromptLocalAsync(pergunta, cancellationToken);

                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraSucesso(pergunta, resposta, tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return Ok(ollamaResponseDto);
            }
            catch (TimeoutException)
            {
                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraErro(pergunta, "Timeout ao chamar Ollama.", tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return BadRequest(ollamaResponseDto);
            }
            catch (Exception ex)
            {
                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraErro(pergunta, ex.Message, tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return BadRequest(ollamaResponseDto);
            }
        }

        [HttpPost("PerguntarComContexto")]
        public async Task<IActionResult> PerguntarComContexto([FromQuery] string pergunta, CancellationToken cancellationToken)
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

                string prompt = _contextoServico.ObterPromptComBaseDocumentos(pergunta, cancellationToken);
                if (string.IsNullOrEmpty(prompt))
                {
                    tempo.Stop();
                    ollamaResponseDto = new OllamaResponseDto().GeraErro(pergunta, "Sem resultado para o assunto informado!", tempo.ElapsedMilliseconds);
                    _helper.Informacao($"{ollamaResponseDto}");
                    return BadRequest(ollamaResponseDto);
                }

                var resposta = await _OllamaServico.ProcessaPromptLocalContextoAsync(prompt, cancellationToken);

                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraSucesso(pergunta, resposta, tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return Ok(ollamaResponseDto);
            }
            catch (TimeoutException)
            {
                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraErro(pergunta, "Timeout ao chamar Ollama.", tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return BadRequest(ollamaResponseDto);
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
