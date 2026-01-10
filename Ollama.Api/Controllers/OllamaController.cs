using Microsoft.AspNetCore.Mvc;
using Ollama.Api.Util;
using Ollama.Aplicacao.Dto;
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
        private readonly HelperConsoleColor _HelperConsoleColor;

        private readonly EngenhariaPromptServico _EngenhariaPromptServico;

        public OllamaController(OllamaServico ollamaServico, EngenhariaPromptServico engenhariaPromptServico, ILogger<OllamaController> logger, HelperConsoleColor helperConsoleColor)
        {
            _OllamaServico = ollamaServico;
            _logger = logger;
            _HelperConsoleColor = helperConsoleColor;
            _EngenhariaPromptServico = engenhariaPromptServico;
        }


        [HttpGet("PerguntaEmGeral")]
        public async Task<IActionResult?> PerguntaEmGeral([FromQuery] string prompt, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, prompt, tempo);
            if (erro != null)
                return erro;

            var resultado = await ResponseHelper.ProcessaPrompt(_OllamaServico, prompt, this, _HelperConsoleColor, tempo, cancellationToken);
            return resultado;

        }



        [HttpGet("ObterPorAssunto")]
        public async Task<IActionResult?> ObterPorTexto([FromQuery] string pergunta, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, pergunta, tempo);
            if (erro != null)
                return erro;

            string prompt = _EngenhariaPromptServico.ObterPromptComBaseDocumentos(pergunta, cancellationToken);
            erro = ResponseHelper.ValidarRetornoPergunta(this, _HelperConsoleColor, prompt, tempo);
            if (erro != null)
                return erro;

            var resultado = await ResponseHelper.ProcessaPrompt(_OllamaServico, prompt, this, _HelperConsoleColor, tempo, cancellationToken);
            return resultado;
        }


        [HttpPost("ObterProOrdemServico")]
        public async Task<IActionResult?> ObterProOrdemServico([FromQuery] string manutentor, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, manutentor, tempo);
            if (erro != null)
                return erro;

            string prompt = _EngenhariaPromptServico.PromptOrdemServico(manutentor);
           // string prompt = promptDto.FormataToString();
            erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, prompt, tempo);
            if (erro != null)
                return erro;

            var resultado = await ResponseHelper.ProcessaPrompt(_OllamaServico, prompt, this, _HelperConsoleColor, tempo, cancellationToken);
            return resultado;
        }


        [HttpPost("EspecialistaOrdemServicoHtml")]
        public async Task<IActionResult?> EspecialistaOrdemServicoHtml([FromQuery] string manutentor, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, manutentor, tempo);
            if (erro != null)
                return erro;

            string prompt = _EngenhariaPromptServico.PromptOrdemServicoHtml(manutentor);
            // string prompt = promptDto.FormataToString();
            erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, prompt, tempo);
            if (erro != null)
                return erro;

            var resultado = await ResponseHelper.ProcessaPrompt(_OllamaServico, prompt, this, _HelperConsoleColor, tempo, cancellationToken);
            return resultado;

        }
    }
}
