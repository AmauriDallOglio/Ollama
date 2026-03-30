using Ollama.Api.Util;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Util;

namespace Ollama.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ILogger logPipelineBuilder = Configuracao.LogPipelineBuilder(builder);
 
            // configura logging nativo .NET
            Configuracao.ConfigurarDotNetLogging(builder);

            // registra serviços personalizados
            Configuracao.RegistrarServicos(builder);

            // Adiciona controladores
            builder.Services.AddControllers();

            //Swagger configurado com título e descrição
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "Ollama API",
                    Version = "v1",
                    Description = "API de integração com o Ollama (Llama3.2)"
                });
            });

            // Registra o IHttpClientFactory
            builder.Services.AddHttpClient();

            // Habilita Swagger para teste
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Adiciona CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

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


            var helper = app.Services.GetRequiredService<HelperConsoleColor>();
            helper.Informacao("Aplicação iniciando...");

   
            app.UseSwagger();
            app.UseSwaggerUI();

            // app.UseHttpsRedirection(); //HTTPS
            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            app.UseMiddleware<ErrorMiddleware>();

            app.Run();
        }
    }
}
