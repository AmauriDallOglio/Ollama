using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;

namespace Ollama.Api.Util
{
    public static class Configuracao
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

            _loggerDotNet.LogWarning(" ***** Amauri Versão 1.0 ***** ");
            _loggerDotNet.LogInformation(" Logging nativo configurado para o builder.");
            _loggerDotNet.LogWarning(" Pipeline do builder iniciado.");
            _loggerDotNet.LogInformation(" Azure Log Diagnostics configurado.");

            return _loggerDotNet;
        }





        // ================================================================
        //  NOVO MÉTODO — REGISTRO DE SERVIÇOS (exclusivo!)
        // ================================================================

        public static void RegistrarServicos(WebApplicationBuilder builder)
        {
            // Serviços para RAG e aprendizado de máquina
            builder.Services.AddSingleton<SessaoMemoriaServico>();


            //// Configurações do appsettings.json
            builder.Services.Configure<AppSettingsDto>(builder.Configuration);

            //builder.Services.Configure<AppSettingsDto>("TipoServidor", builder.Configuration.GetSection("TipoServidor"));
            //builder.Services.Configure<AppSettingsDto>("Local", builder.Configuration.GetSection("OllamaLocal"));
            //builder.Services.Configure<AppSettingsDto>("Docker", builder.Configuration.GetSection("OllamaDocker"));

            //Desabilitar totalmente o ModelState automático do .NET para permitir validação e tratamento manual
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddTransient<HelperConsoleColor>();
            builder.Services.AddHttpClient<OllamaServico>();
            builder.Services.AddSingleton<EngenhariaPromptDocumentos>(sp =>
            {
                var docs = new List<DocumentoContextoDto>
                {
                    new DocumentoContextoDto("1", "Batman", "É um super-herói da DC conhecido como o Cavaleiro das Trevas, que protege Gotham City."),
                    new DocumentoContextoDto("2", "Superman", "É um super-herói da DC vindo de Krypton, com poderes como superforça, visão de calor e voo."),
                    new DocumentoContextoDto("3", "Mulher-Maravilha", "É uma amazona guerreira da DC, com força sobre-humana e o Laço da Verdade."),
                    new DocumentoContextoDto("4", "Flash", "É o velocista escarlate da DC, capaz de correr em velocidades incríveis e manipular o tempo."),
                    new DocumentoContextoDto("5", "Aquaman", "É o rei de Atlântida na DC, com poderes de controlar o mar e se comunicar com criaturas marinhas."),
                };

                return new EngenhariaPromptDocumentos(docs );
            });
            builder.Services.AddSingleton<SessaoMemoriaDto>();
            builder.Services.AddSingleton<EngenhariaPromptBase>();
            builder.Services.AddSingleton<EngenhariaPromptDadosMocados>();

 


        }

        // ================================================================
        // Abre um canal para registrar o ILogger na classe
        // ================================================================

        public static void RegistrarLoggerViaApp(WebApplication app)
        {
            // obtém helper do container DI
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            RegistrarLogger(logger);

            _loggerDotNet?.LogInformation("-------------------------------------");

            _loggerDotNet?.LogInformation(" Migrando do logger temporário para o logger final.");
            _loggerDotNet?.LogInformation("-------------------------------------");

        }

        private static void RegistrarLogger(ILogger logger)
        {
            _loggerDotNet = logger;
        }
    }
}
