using Ollama.Aplicacao.Dto;
using System.Diagnostics;

namespace Ollama.Api.Util
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var tempo = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            catch (TimeoutException ex)
            {
                tempo.Stop();
                _logger.LogError(ex, "Timeout na requisição");

                var response = new ResultadoOperacaoDto().GeraErro( context.Request.Path, $"Timeout ao chamar Ollama. {ex.Message}",    tempo.ElapsedMilliseconds);

                context.Response.StatusCode = StatusCodes.Status408RequestTimeout;
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                tempo.Stop();
                _logger.LogError(ex, "Erro inesperado");

                var response = new ResultadoOperacaoDto().GeraErro(  context.Request.Path, ex.Message, tempo.ElapsedMilliseconds);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
