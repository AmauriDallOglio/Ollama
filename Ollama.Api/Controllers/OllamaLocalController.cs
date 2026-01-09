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
    public class OllamaLocalController : ControllerBase
    {
        private readonly ILogger<OllamaLocalController> _logger;
        private readonly OllamaServico _OllamaServico;
        private readonly HelperConsoleColor _helper;
        private readonly PromptDocumentoServico _contextoServico;

        public OllamaLocalController(OllamaServico ollamaServico, PromptDocumentoServico contextoServico, ILogger<OllamaLocalController> logger, HelperConsoleColor helper)
        {
            _OllamaServico = ollamaServico;
            _logger = logger;
            _helper = helper;
            _contextoServico = contextoServico;
        }


        [HttpGet("PerguntaEmGeral")]
        public IActionResult PerguntaEmGeral([FromQuery] string pergunta, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var erro = ResponseHelper.ValidarPergunta(this, _helper, pergunta, tempo);
            if (erro != null)
                return erro;

            Func<Task<string>> acao = () => _OllamaServico.ProcessaPromptAsync(pergunta, cancellationToken);

            return ResponseHelper.CriarResposta(this, _helper, pergunta, acao, tempo);
        }



        [HttpGet("PerguntarComContexto")]
        public IActionResult PerguntarComContexto([FromQuery] string pergunta, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var erro = ResponseHelper.ValidarPergunta(this, _helper, pergunta, tempo);
            if (erro != null)
                return erro;

            string prompt = _contextoServico.ObterPromptComBaseDocumentos(pergunta, cancellationToken);
            erro = ResponseHelper.ValidarPergunta(this, _helper, prompt, tempo);
            if (erro != null)
                return erro;

            Func<Task<string>> acao = () => _OllamaServico.ProcessaPromptAsync(prompt, cancellationToken);

            return ResponseHelper.CriarResposta(this, _helper, pergunta, acao, tempo);
        }



        [HttpPost("EspecialistaOrdemServico")]
        public async Task<IActionResult> EspecialistaOrdemServico([FromQuery] string manutentor, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var erro = ResponseHelper.ValidarPergunta(this, _helper, manutentor, tempo);
            if (erro != null)
                return erro;

            PromptResponseDto promptDto = new EngenhariaPromptServico().PromptOrdemServico(manutentor);
            string prompt = promptDto.FormataToString();
            erro = ResponseHelper.ValidarPergunta(this, _helper, prompt, tempo);
            if (erro != null)
                return erro;

            Func<Task<string>> acao = () => _OllamaServico.ProcessaPromptAsync(prompt, cancellationToken);

            _helper.Informacao($"{acao().GetAwaiter().GetResult()}");
            return Ok(acao().GetAwaiter().GetResult());

            //return ResponseHelper.CriarResposta(this, _helper, prompt, acao, tempo);
        }


        [HttpPost("EspecialistaOrdemServicoHtml")]
        public async Task<IActionResult> EspecialistaOrdemServicoHtml([FromQuery] string manutentor, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var erro = ResponseHelper.ValidarPergunta(this, _helper, manutentor, tempo);
            if (erro != null)
                return erro;

            PromptResponseDto promptDto = new EngenhariaPromptServico().PromptOrdemServicoHtml(manutentor);
            string prompt = promptDto.FormataToString();
            erro = ResponseHelper.ValidarPergunta(this, _helper, prompt, tempo);
            if (erro != null)
                return erro;

            Func<Task<string>> acao = () => _OllamaServico.ProcessaPromptAsync(prompt, cancellationToken);

            _helper.Informacao($"{acao().GetAwaiter().GetResult()}");
            return Ok(acao().GetAwaiter().GetResult());

            //return ResponseHelper.CriarResposta(this, _helper, prompt, acao, tempo);

        }


 



    }
}
