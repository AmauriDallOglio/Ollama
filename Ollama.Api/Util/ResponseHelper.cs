using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using System.Diagnostics;

namespace Ollama.Api.Util
{
    public static class ResponseHelper
    {


        public static IActionResult? ValidarPergunta(ControllerBase controller, HelperConsoleColor helperConsoleColor, string pergunta, Stopwatch tempo)
        {
            if (string.IsNullOrWhiteSpace(pergunta))
            {
                tempo.Stop();
                var dto = new ResultadoOperacaoDto().GeraErro(pergunta, "Campos devem ser informados!", tempo.ElapsedMilliseconds);

                helperConsoleColor.Alerta($"{dto.Resposta}");
                return controller.BadRequest(dto);
            }
            return null!;
        }

        public static IActionResult? ValidarRetornoPergunta(ControllerBase controller, HelperConsoleColor helperConsoleColor, string prompt, Stopwatch tempo)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                tempo.Stop();
                var dto = new ResultadoOperacaoDto().GeraErro(prompt, "Desculpe, não encontrei informações sobre isso na minha base de dados.", tempo.ElapsedMilliseconds);

                helperConsoleColor.Alerta($"{dto.Resposta}");
                return controller.BadRequest(dto);
            }
            return null!;
        }


        public static async Task<IActionResult?> ProcessaPrompt(OllamaServico ollamaServico, string prompt, ControllerBase controller, HelperConsoleColor helperConsoleColor, Stopwatch tempo, CancellationToken cancellationToken)
        {
            
            var resposta = await ollamaServico.ProcessaPromptAsync(prompt, cancellationToken);
           
            tempo.Stop();

            var dto = new ResultadoOperacaoDto().GeraSucesso(prompt, resposta, tempo.ElapsedMilliseconds);

            helperConsoleColor.Alerta($"{dto.Pergunta}");
            helperConsoleColor.Alerta($"{dto.Resposta}");
            return controller.Ok(dto);
        }

    }
}
