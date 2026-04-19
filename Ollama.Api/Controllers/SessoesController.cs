using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Rotas.SessaoRota;
using Ollama.Aplicacao.Util;
using Ollama.Dominio.InterfaceRepositorio;


namespace Ollama.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessoesController : ControllerBase
    {
    
        private readonly ISessaoCommandRepositorio _ISessaoCommandRepositorio;
        private readonly IContratoBaseHandler<ObterTodosSessaoRequest, ResultadoOperacao> _handler;
        public SessoesController(
             ISessaoCommandRepositorio iSessaoCommandRepositorio,
             IContratoBaseHandler<ObterTodosSessaoRequest, ResultadoOperacao> handler)
        {
            _ISessaoCommandRepositorio = iSessaoCommandRepositorio;
            _handler = handler;
        }



        [AllowAnonymous]
        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos([FromQuery] ObterTodosSessaoRequest request, CancellationToken cancellationToken)
        {
            var resultado = await _handler.Executar(request, cancellationToken);
            return Ok(resultado);
        }
    }
}
