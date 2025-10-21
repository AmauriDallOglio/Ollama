using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Servico;

namespace Ollama.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurações do appsettings.json
            builder.Services.Configure<OllamaAppSettingsDto>(builder.Configuration.GetSection("Ollama"));

            // Adiciona HttpClient para o serviço OllamaService
            builder.Services.AddHttpClient<OllamaServico>();

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



            var app = builder.Build();
 
 


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
