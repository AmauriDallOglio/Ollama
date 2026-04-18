using Ollama.Aplicacao.Util;

namespace Ollama.Api.Configuracao
{
    public static class LoggerConfiguracao
    {
        private static ILogger? _loggerDotNet;


        // ================================================================
        //  CONFIGURAÇÃO DO LOG NATIVO .NET
        // ================================================================
        public static void ConfigurarDotNetLogging(WebApplicationBuilder builder)
        {
            // ----------------------------------------------
            //  CONFIGURAÇÃO DE LOG DO BUILDER
            // ----------------------------------------------
            // Remove provedores padrão
            builder.Logging.ClearProviders();

            // Console local
            builder.Logging.AddConsole();

            // Debug (somente ambiente local)
            builder.Logging.AddDebug();

            // Azure Diagnostics (envia para Log Stream)
            //Instalar Microsoft.Extensions.Logging.AzureAppServices" Version="8.0.22"
            builder.Logging.AddAzureWebAppDiagnostics();

            // Application Insights (telemetria + logs + métricas)
            //Instalar Microsoft.ApplicationInsights.AspNetCore" Version="2.23.0" 
            builder.Services.AddApplicationInsightsTelemetry();

            //  Configurar níveis de log por categoria
            builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
            builder.Logging.AddFilter("System", LogLevel.Warning);

            // Configurar seu projeto inteiro
            builder.Logging.AddFilter("Serialog", LogLevel.Information);


            //  Ou nível global
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
            // builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Query", LogLevel.Debug);


            //// ----------------------------------------------
            //// CRIAR LOGGER TEMPORÁRIO MANUAL
            //// (funciona ANTES do app.Build())
            //// ----------------------------------------------


            //_loggerTemp = LoggerFactory.Create(logging =>
            //{
            //    logging.AddConsole();
            //    logging.AddDebug();
            //}).CreateLogger("PipelineBuilder");

            //_loggerTemp.LogWarning(" ***** Amauri Versão 1.0 ***** ");
            //_loggerTemp.LogInformation(" Logging nativo configurado para o builder.");
            //_loggerTemp.LogWarning(" Pipeline do builder iniciado.");
            //_loggerTemp.LogInformation(" Azure Log Diagnostics configurado.");

        }

        // ================================================================
        //  NOVO MÉTODO — REGISTRO DE SERVIÇOS (exclusivo!)
        // ================================================================
        public static ILogger LogPipelineBuilder(WebApplicationBuilder builder)
        {
            _loggerDotNet = LoggerFactory.Create(logging =>
            {
                logging.AddConsole();
                logging.AddDebug();
            }).CreateLogger("LogPipelineBuilder");

            PrintaConsole.ConfigurarLogger(_loggerDotNet);

            _loggerDotNet.LogWarning("  Teste do alerta ");
            _loggerDotNet.LogInformation(" Teste do info  ");
            _loggerDotNet.LogError(" Teste do error ");
 

            // Logs via PrintaConsole (coloridos + ILogger)
            PrintaConsole.Alerta(" Teste do alerta ");
            PrintaConsole.Info(" Teste do info ");
            PrintaConsole.Error(" Teste do error ");
 

            return _loggerDotNet;
        }

    }
}
