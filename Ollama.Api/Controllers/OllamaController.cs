using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using System.Diagnostics;
using static Ollama.Aplicacao.Servico.ContextoServico;

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

        [HttpGet("Perguntar")]
        public async Task<IActionResult> Perguntar([FromQuery] string texto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(texto))
                    return BadRequest(new { erro = "Assunto obrigatório." });

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
        public async Task<IActionResult> PerguntarComContexto([FromQuery] PerguntaRequest req, CancellationToken ct)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(req.Assunto))
                    return BadRequest(new { erro = "Assunto obrigatório." });

                var resp = await _OllamaServico.PerguntarComContextoAsync(req.Assunto, req.TopK, ct);
                _helper.Informacao($"{resp}");
                return Ok(new { sucesso = true, resp.Resposta, tempoMs = resp.TempoMs });
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
    }
}
