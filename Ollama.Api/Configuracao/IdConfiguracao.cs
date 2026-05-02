using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Rotas.DocumentoRota;
using Ollama.Aplicacao.Rotas.OllamaRota;
using Ollama.Aplicacao.Rotas.SessaoRota;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using Ollama.Dominio.InterfaceRepositorio;
using Ollama.Infraestrutura.Repositorio;
using Ollama.Servico.Ollama;
using Ollama.Servico.Ollama.Interface;

namespace Ollama.Api.Configuracao
{
    public static class IdConfiguracao
    {


        // ================================================================
        //  NOVO MÉTODO — REGISTRO DE SERVIÇOS (exclusivo!)
        // ================================================================

        public static void RegistrarServicos(WebApplicationBuilder builder)
            
        {
            //Aplicacao
            builder.Services.AddScoped<IContratoBaseHandler<ObterTodosSessaoRequest, ResultadoOperacao>, ObterTodosSessaoHandler >();
            builder.Services.AddScoped<IContratoBaseHandler<ObterTodosDocumentoRequest, ResultadoOperacao>, ObterTodosDocumentoHandler>();
            builder.Services.AddScoped<IContratoBaseHandler<ImportarDocumentoRequest, ResultadoOperacao>, ImportarDocumentoHandler>();
            builder.Services.AddScoped<IContratoBaseHandler<PromptRequest, ResultadoOperacao>, PromptHandler>();

            builder.Services.AddScoped<PromptGenerativoDadosMocadosHandler>();
            builder.Services.AddScoped<PromptHandler>();
            builder.Services.AddScoped<PromptGenerativoHandler>();
 


            // Serviços para RAG e aprendizado de máquina
            builder.Services.AddSingleton<ISessaoMemoriaServico, SessaoMemoriaServico>();

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddScoped<HelperConsoleColor>();
            builder.Services.AddHttpClient<OllamaServico>();
            builder.Services.AddScoped<IOllamaServico, OllamaServico>();
 
    

            builder.Services.AddScoped<ISessaoCommandRepositorio, SessaoCommandRepositorio>();
            builder.Services.AddScoped<IDocumentoCommandRepositorio, DocumentoCommandRepositorio>();
     


        }
    }
}
