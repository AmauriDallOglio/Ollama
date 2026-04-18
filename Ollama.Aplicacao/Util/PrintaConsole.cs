using Microsoft.Extensions.Logging;

namespace Ollama.Aplicacao.Util
{
    public static class PrintaConsole
    {
        private static ILogger? _logger;

        // Método para configurar o logger
        public static void ConfigurarLogger(ILogger logger)
        {
            _logger = logger;
        }

        private static void Padrao(string mensagem, ConsoleColor foreground, ConsoleColor background, LogLevel level)
        {
            Console.BackgroundColor = background;
            Console.ForegroundColor = foreground;
            Console.WriteLine(mensagem);
            Console.ResetColor();

            // Se o logger estiver configurado, registra também
            _logger?.Log(level, mensagem);
        }

        public static void Error(string mensagem)
        {
            Padrao($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {mensagem}", ConsoleColor.White, ConsoleColor.Red, LogLevel.Error);
        }

        public static void Sucesso(string mensagem)
        {
            Padrao($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {mensagem}", ConsoleColor.Black, ConsoleColor.Green, LogLevel.Information);
        }

        public static void Alerta(string mensagem)
        {
            Padrao($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {mensagem}", ConsoleColor.Black, ConsoleColor.Yellow, LogLevel.Warning);
        }

        public static void Info(string mensagem)
        {
            Padrao($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {mensagem}", ConsoleColor.Yellow, ConsoleColor.Blue, LogLevel.Information);
        }

    }
}
