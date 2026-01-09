using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using System.Diagnostics;

namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OllamaPromptController : ControllerBase
    {
        private readonly ILogger<OllamaPromptController> _logger;
        private readonly OllamaServico _OllamaServico;
        private readonly HelperConsoleColor _helper;

        public OllamaPromptController(OllamaServico ollamaServico, ILogger<OllamaPromptController> logger, HelperConsoleColor helper)
        {
            _OllamaServico = ollamaServico;
            _logger = logger;
            _helper = helper;   
        }



        [HttpPost("PerguntaEmGeral")]
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

                string resposta = await _OllamaServico.ProcessaPromptLocalContextoAsync(pergunta, OllamaServico.TipoServidor.ServidorLocal, cancellationToken);

                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraSucesso(pergunta, resposta, tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return Ok(ollamaResponseDto);
            }
            catch (Exception ex)
            {
                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraErro(pergunta, $"Ocorreu um erro ao se comunicar com a servidor Ollama local. {ex.Message} ", tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return BadRequest(ollamaResponseDto);
            }
        }


        [HttpPost("EspecialistaOrdemServico")]
        public async Task<IActionResult> EspecialistaOrdemServico([FromQuery] string manutentor, CancellationToken cancellationToken)
        {
            OllamaResponseDto ollamaResponseDto = new();
            var tempo = Stopwatch.StartNew();
            try
            {
 
                if (string.IsNullOrWhiteSpace(manutentor))
                {
                    tempo.Stop();
                    ollamaResponseDto = new OllamaResponseDto().GeraErro(manutentor, "O nome do manutentor não pode ser vazio.", tempo.ElapsedMilliseconds);
                    _helper.Informacao($"{ollamaResponseDto}");
                    return BadRequest(ollamaResponseDto);
                }


                PromptResponseDto promptDto = new EngenhariaPromptServico().PromptOrdemServico(manutentor);

                string texto = promptDto.FormataToString();
                string resposta = await _OllamaServico.ProcessaPromptLocalContextoAsync(texto, OllamaServico.TipoServidor.ServidorLocal, cancellationToken);

                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraSucesso(manutentor, resposta, tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return Ok(ollamaResponseDto);
            }
            catch (Exception ex)
            {
                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraErro(manutentor, $"Ocorreu um erro ao se comunicar com a servidor Ollama local. {ex.Message} ", tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return BadRequest(ollamaResponseDto);
            }



           

        }


        [HttpPost("EspecialistaOrdemServicoHtml")]
        public async Task<IActionResult> EspecialistaOrdemServicoHtml([FromQuery] string manutentor, CancellationToken cancellationToken)
        {
            OllamaResponseDto ollamaResponseDto = new();
            var tempo = Stopwatch.StartNew();
            try
            {
                if (string.IsNullOrWhiteSpace(manutentor))
                {
                    tempo.Stop();
                    ollamaResponseDto = new OllamaResponseDto().GeraErro(manutentor, "O nome do manutentor não pode ser vazio.", tempo.ElapsedMilliseconds);
                    _helper.Informacao($"{ollamaResponseDto}");
                    return BadRequest(ollamaResponseDto);
                }


                PromptResponseDto promptDto = new EngenhariaPromptServico().PromptOrdemServicoHtml(manutentor);

                string texto = promptDto.FormataToString();
                string resposta = await _OllamaServico.ProcessaPromptLocalContextoAsync(texto, OllamaServico.TipoServidor.ServidorLocal, cancellationToken);

                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraSucesso(manutentor, resposta, tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return Ok(ollamaResponseDto);
            }
            catch (Exception ex)
            {
                tempo.Stop();
                ollamaResponseDto = new OllamaResponseDto().GeraErro(manutentor, $"Ocorreu um erro ao se comunicar com a servidor Ollama local. {ex.Message} ", tempo.ElapsedMilliseconds);
                _helper.Informacao($"{ollamaResponseDto}");
                return BadRequest(ollamaResponseDto);
            }

        }

    }
}
