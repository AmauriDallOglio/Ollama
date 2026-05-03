using Ollama.Api.Configuracao;
using Ollama.Api.Util;
using Ollama.Aplicacao.Util;

namespace Ollama.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var ambiente = builder.Environment.IsDevelopment() ? "appsettings.Development.json" : "appsettings.json";
            PrintaConsole.Alerta($"Configuração: {ambiente}");
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile(ambiente, optional: false, reloadOnChange: true)
                .Build();



            PrintaConsole.Info("Carregando configuração do Logging");
            LoggerConfiguracao.ConfigurarDotNetLogging(builder);
         //   ILogger logPipelineBuilder = LoggerConfiguracao.LogPipelineBuilder(builder);


            PrintaConsole.Info("Carregando appsettings");
            AppSettingsConfiguracao.Carregar(builder.Services, configuration);


            PrintaConsole.Info("Carregando injeção dependência");
            IdConfiguracao.RegistrarServicos(builder);

            PrintaConsole.Info("Carregando configuração do Swagger");
            ConfiguracaoApi.ConfiguracaoSwagger(builder.Services);

            PrintaConsole.Info("Carregando tarefa");
            builder.Services.AddHostedService<TarefaSessaoMemoria>();


            var app = builder.Build();

            // redireciona "/" para o Swagger
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger/index.html");
                    return;
                }
                await next();
            });

            PrintaConsole.Alerta("Aplicação iniciando");
            app.UseMiddleware<ErrorMiddleware>();
            PrintaConsole.Alerta("Iniciando UseSwagger");
            app.UseSwagger();
            PrintaConsole.Alerta("Iniciando UseSwaggerUI");
            app.UseSwaggerUI();
            PrintaConsole.Alerta("Iniciando UseCors");
            app.UseCors("AllowAll");
            PrintaConsole.Alerta("Iniciando UseAuthorization");
            app.UseAuthorization();
            PrintaConsole.Alerta("Iniciando Prometheus");
            app.MapPrometheusScrapingEndpoint().AllowAnonymous();

            PrintaConsole.Alerta("Iniciando MapControllers");
            app.MapControllers();
            PrintaConsole.Alerta("Iniciando Run");
            app.Run();
        }
    }
}
