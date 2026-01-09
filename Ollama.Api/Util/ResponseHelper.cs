using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Util;
using System.Diagnostics;

namespace Ollama.Api.Util
{
    public static class ResponseHelper
    {
 

        public static IActionResult CriarResposta(ControllerBase controller, HelperConsoleColor helper, string pergunta, Func<Task<string>> acao, Stopwatch tempo)
        {
        
            string resposta = acao().GetAwaiter().GetResult();

            tempo.Stop();
            OllamaResponseDto dto = new OllamaResponseDto().GeraSucesso(pergunta, resposta, tempo.ElapsedMilliseconds);

            helper.Informacao($"{acao().GetAwaiter().GetResult()}");

            return controller.Ok(dto);
        }

        public static IActionResult? ValidarPergunta(ControllerBase controller, HelperConsoleColor helper, string pergunta, Stopwatch tempo)
        {
            if (string.IsNullOrWhiteSpace(pergunta))
            {
                tempo.Stop();
                var dto = new OllamaResponseDto().GeraErro(pergunta, "Campos devem ser informados!", tempo.ElapsedMilliseconds);

                helper.Informacao($"{dto}");
                return controller.BadRequest(dto);
            }
            return null!;
        }
    }
}
