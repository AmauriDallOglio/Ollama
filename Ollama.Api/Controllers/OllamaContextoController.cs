using Microsoft.AspNetCore.Mvc;
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
            try
            {
                var tempo = Stopwatch.StartNew();

                if (string.IsNullOrWhiteSpace(pergunta))
                    return BadRequest(new { erro = "Assunto obrigatório." });

     
                if (string.IsNullOrWhiteSpace(pergunta))
                {
                    string mensagem = "Informe uma pergunta válida.";

                    _helper.Erro($"{mensagem}");
                    return BadRequest(new { erro = mensagem });
                }

                var resposta = await _OllamaServico.ProcessaPromptLocalAsync(pergunta, cancellationToken);

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
            catch (TimeoutException)
            {
                return StatusCode(504, new { sucesso = false, mensagem = "Timeout ao chamar Ollama." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = ex.Message });
            }
        }

        [HttpPost("PerguntarComContexto")]
        public async Task<IActionResult> PerguntarComContexto([FromQuery] string pergunta, CancellationToken cancellationToken)
        {
            try
            {
                var tempo = Stopwatch.StartNew();
                if (string.IsNullOrWhiteSpace(pergunta))
                    return BadRequest(new { erro = "Assunto obrigatório." });

                string prompt = _contextoServico.ObterPromptComBaseDocumentos(pergunta, cancellationToken);
                if (string.IsNullOrEmpty(prompt))
                {
                    return BadRequest(new { erro = "Sem resultado para o assunto informado!" });
                }

                var resp = await _OllamaServico.ProcessaPromptLocalContextoAsync(prompt, cancellationToken);
                _helper.Informacao($"{resp}");
                tempo.Stop();

                return Ok(new { Sucesso = true, resp, Tempo = $"{tempo.ElapsedMilliseconds} ms" });
            }
            catch (TimeoutException)
            {
                return StatusCode(504, new { Sucesso = false, mensagem = "Timeout ao chamar Ollama.", Tempo = 0 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { sucesso = false, mensagem = ex.Message, Tempo = 0 });
            }
        }
    }
}
