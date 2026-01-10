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
    public class SessaoMemoriaController : ControllerBase
    {
        private readonly SessaoMemoria _SessaoMemoria;
        private readonly OllamaServico _OllamaServico;
        private readonly HelperConsoleColor _HelperConsoleColor;
        private readonly EngenhariaPromptServico _EngenhariaPromptServico;

        public SessaoMemoriaController(OllamaServico ollamaServico, EngenhariaPromptServico engenhariaPromptServico, SessaoMemoria sessaoMemoria, HelperConsoleColor helperConsoleColor)
        {
            _OllamaServico = ollamaServico;
            _HelperConsoleColor = helperConsoleColor;
            _EngenhariaPromptServico = engenhariaPromptServico;
            _SessaoMemoria = sessaoMemoria;
        }

        [HttpPost("GravarSessao")]
        public IActionResult GravarSessao([FromBody] string texto)
        {
            var textoExistente = _SessaoMemoria.Obter("sessao_amauri");

            if (textoExistente == null)
            {
                _SessaoMemoria.Gravar("sessao_amauri", texto);
                return Ok(new { Sucesso = true, Acao = "Gravado", Conteudo = texto });
            }
            else
            {
                _SessaoMemoria.Atualizar("sessao_amauri", texto);
                return Ok(new { Sucesso = true, Acao = "Atualizado", Conteudo = texto });
            }
        }


        [HttpGet("ObterSessaoGravada")]
        public IActionResult ObterSessaoGravada()
        {
            var texto = _SessaoMemoria.Obter("sessao_amauri");
            if (texto == null)
                return NotFound("Sessão não encontrada.");

            return Ok(new { Sucesso = true, Sessao = "sessao_amauri", Conteudo = texto });
        }


        [HttpGet("ObterComPergunta")]
        public async Task<IActionResult?> ObterComPergunta([FromQuery] string pergunta, CancellationToken cancellationToken)
        {
            var tempo = Stopwatch.StartNew();

            var texto = _SessaoMemoria.Obter("sessao_amauri");
            if (texto == null)
                return NotFound("Sessão não encontrada.");

            string prompt = _EngenhariaPromptServico.PromptSessao(pergunta, texto);
            //string prompt = promptDto.FormataToString();
            var erro = ResponseHelper.ValidarPergunta(this, _HelperConsoleColor, prompt, tempo);
            if (erro != null)
                return erro;

            var resultado = await ResponseHelper.ProcessaPrompt(_OllamaServico, prompt, this, _HelperConsoleColor, tempo, cancellationToken);
            return resultado;
        }


        [HttpPut("Atualizar")]
        public IActionResult Atualizar([FromBody] string novoTexto)
        {
            _SessaoMemoria.Atualizar("sessao_amauri", novoTexto);
            return Ok(new { Sucesso = true, Atualizado = novoTexto });
        }


        [HttpDelete("Excluir")]
        public IActionResult Excluir()
        {
            _SessaoMemoria.Excluir("sessao_amauri");
            return Ok(new { Sucesso = true, Mensagem = "Sessão excluída." });
        }



    }
}

