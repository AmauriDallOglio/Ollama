using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Rotas.OllamaRota;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using Ollama.Servico.Ollama;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaController : ControllerBase
    {
        private readonly ILogger<OllamaController> _logger;
        private readonly IOllamaServico _iOllamaServico;
        private readonly HelperConsoleColor _HelperConsoleColor;
        private readonly IEngenhariaPromptDocumentos _EngenhariaPromptServico;
        private readonly EngenhariaPromptBase _EngenhariaPromptBase;
        private readonly EngenhariaPromptDadosMocados _EngenhariaPromptDadosMocados;
        private readonly SessaoMemoriaServico _servicoLogInteracao;
        private readonly IWebHostEnvironment _env;
        private readonly PromptHandler _PromptHandler;
        private readonly PromptGenerativoHandler _PromptGenerativoHandler;
        public OllamaController( 
                                IEngenhariaPromptDocumentos engenhariaPromptServico, 
                                EngenhariaPromptBase engenhariaPromptBase,
                                EngenhariaPromptDadosMocados engenhariaPromptDadosMocados,
                                ILogger<OllamaController> logger,
                                SessaoMemoriaServico servicoLogInteracao,
                                HelperConsoleColor helperConsoleColor,
                                IWebHostEnvironment env,
                                PromptHandler promptHandler,
                                IOllamaServico iOllamaServico,
                                PromptGenerativoHandler promptGenerativoHandler
            )
        {
            _iOllamaServico = iOllamaServico;
            _logger = logger;
            _HelperConsoleColor = helperConsoleColor;
            _EngenhariaPromptServico = engenhariaPromptServico;
            _EngenhariaPromptBase = engenhariaPromptBase;
            _EngenhariaPromptDadosMocados = engenhariaPromptDadosMocados;
            _servicoLogInteracao = servicoLogInteracao;
            _env = env;
            _PromptHandler = promptHandler;
            _PromptGenerativoHandler = promptGenerativoHandler;
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

        //[HttpGet("PromptGenerativo")]
        //public async Task<IActionResult?> PromptGenerativo([FromQuery] string pergunta, CancellationToken cancellationToken)
        //{
        //    var tempo = Stopwatch.StartNew();

        //    if (string.IsNullOrWhiteSpace(pergunta))
        //    {
        //        tempo.Stop();
        //        ResultadoOperacaoDto dto = new ResultadoOperacaoDto().GeraErro(pergunta, "Campos devem ser informados!", tempo.ElapsedMilliseconds);

        //        _HelperConsoleColor.Erro($"{dto.Resposta}");
        //        return BadRequest(dto);
        //    }

        //    string resposta = await _EngenhariaPromptServico.ObterPromptComBaseDocumentos(pergunta, cancellationToken);
        //    ResultadoOperacaoDto resultado = new ResultadoOperacaoDto();
        //    if (!string.IsNullOrEmpty(resposta))
        //    {
        //        resposta = await _iOllamaServico.ProcessaPerguntaRagAsync(resposta, "Sistema", cancellationToken);
        //        if (!string.IsNullOrEmpty(resposta))
        //        {
        //            tempo.Stop();
        //            resultado.GeraSucesso(pergunta, resposta, tempo.ElapsedMilliseconds);
        //            _HelperConsoleColor.Sucesso($"{resultado.Pergunta}");
        //            _HelperConsoleColor.Sucesso($"{resultado.Resposta}");
        //            return Ok(resposta);
        //        }
        //        else
        //        {
        //            tempo.Stop();
        //            resultado.GeraErro(pergunta, "Campos devem ser informados!", tempo.ElapsedMilliseconds);
        //            _HelperConsoleColor.Erro($"{resultado.Pergunta}");
        //            _HelperConsoleColor.Erro($"{resultado.Resposta}");
        //            return BadRequest(resposta);
        //        }
        //    }
        //    else
        //    {
        //        tempo.Stop();
        //        resultado.GeraErro(pergunta, "Campos devem ser informados!", tempo.ElapsedMilliseconds);
        //        _HelperConsoleColor.Erro($"{resultado.Pergunta}");
        //        _HelperConsoleColor.Erro($"{resultado.Resposta}");
        //        return BadRequest(resposta);
        //    }
        //}


        [HttpGet("ObterMemoria")]
        public async Task<IActionResult> ObterMemoria(CancellationToken cancellationToken)
        {
            var logs = await _servicoLogInteracao.ObterTodosAsync(cancellationToken);
            return Ok(logs);
        }


        [HttpPost("ImportarArquivo")]
        public async Task<IActionResult> ImportarArquivo(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var caminho = Path.Combine(_env.ContentRootPath, "uploads", arquivo.FileName);

            Directory.CreateDirectory(Path.GetDirectoryName(caminho)!);

            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return Ok(new { mensagem = "Arquivo importado com sucesso", caminho });
        }

        //[HttpGet("PromptGenerativoDados")]
        //public async Task<IActionResult?> PromptGenerativoDados([FromQuery] string pergunta, CancellationToken cancellationToken)
        //{
        //    var tempo = Stopwatch.StartNew();

        //    var erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, pergunta, tempo);
        //    if (erro != null)
        //        return erro;

        //    string prompt = _EngenhariaPromptDadosMocados(pergunta, cancellationToken);
        //    erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, prompt, tempo);
        //    if (erro != null)
        //        return erro;

        //    var resultado = await ResponseHelper.ProcessaPrompt(_OllamaServico, prompt, this, _HelperConsoleColor, tempo, cancellationToken);
        //    return resultado;
        //}


        //[HttpPost("EspecialistaOrdemServicoHtml")]
        //public async Task<IActionResult?> EspecialistaOrdemServicoHtml([FromQuery] string manutentor, CancellationToken cancellationToken)
        //{
        //    var tempo = Stopwatch.StartNew();

        //    var erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, manutentor, tempo);
        //    if (erro != null)
        //        return erro;

        //    string prompt = _EngenhariaPromptDadosMocados(manutentor);
        //    erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, prompt, tempo);
        //    if (erro != null)
        //        return erro;

        //    var resultado = await ResponseHelper.ProcessaPrompt(_OllamaServico, prompt, this, _HelperConsoleColor, tempo, cancellationToken);
        //    return resultado;

        //}
    }
}
