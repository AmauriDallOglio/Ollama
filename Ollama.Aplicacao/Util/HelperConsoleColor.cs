using Microsoft.Extensions.Logging;

namespace Ollama.Aplicacao.Util
{
    public class HelperConsoleColor
    {
        private readonly ILogger<HelperConsoleColor> _logger;

        public HelperConsoleColor(ILogger<HelperConsoleColor> logger)
        {
            _logger = logger;
        }


        // =====================================================================
        //   MÉTODOS DE LOG (unifica Serilog + ILogger + Console Color)
        // =====================================================================

        public void Informacao(string mensagem)
        {
            var mensagemTmp = $"---- {mensagem}";
            //Log.Information(mensagem);
            _logger.LogInformation(mensagemTmp);
            Colorir(mensagemTmp, ConsoleColor.Black, ConsoleColor.Green);
        }

        public void Sucesso(string mensagem)
        {
            var mensagemTmp = $"---- {mensagem}";
            //Log.Information(mensagemTmp);
            _logger.LogInformation(mensagemTmp);
            Colorir(mensagemTmp, ConsoleColor.Black, ConsoleColor.Cyan);
        }

        public void Erro(string mensagem)
        {
            var mensagemTmp = $"---- {mensagem}";
            // Log.Error(mensagemTmp);
            _logger.LogError(mensagemTmp);
            Colorir(mensagemTmp, ConsoleColor.White, ConsoleColor.Red);
        }

        public void Alerta(string mensagem)
        {
            var mensagemTmp = $"---- {mensagem}";
            // Log.Warning(mensagemTmp);
            _logger.LogWarning(mensagemTmp);
            Colorir(mensagemTmp, ConsoleColor.Black, ConsoleColor.Yellow);
        }

        public void Detalhado(string mensagem)
        {
            var mensagemTmp = $"---- {mensagem}";
            //Log.Debug(mensagemTmp);
            _logger.LogDebug(mensagemTmp);
            Colorir(mensagemTmp, ConsoleColor.DarkGray, ConsoleColor.Black);
        }

        private static void Colorir(string mensagem, ConsoleColor fg, ConsoleColor bg)
        {
            Console.BackgroundColor = bg;
            Console.ForegroundColor = fg;
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {mensagem}");
            Console.ResetColor();
        }

    }
}
