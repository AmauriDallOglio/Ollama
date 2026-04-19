using Microsoft.AspNetCore.Mvc;
using Ollama.Aplicacao.Dto;
using Ollama.Aplicacao.Rotas.DocumentoRota;
using Ollama.Aplicacao.Rotas.SessaoRota;
using Ollama.Aplicacao.Servico;
using Ollama.Aplicacao.Util;
using Ollama.Dominio.InterfaceRepositorio;
using Ollama.Infraestrutura.Repositorio;
using Ollama.Servico.Ollama;

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

            // Serviços para RAG e aprendizado de máquina
            builder.Services.AddSingleton<SessaoMemoriaServico>();


            ////// Configurações do appsettings.json
            //builder.Services.Configure<AppSettingsDto>(builder.Configuration);

            ////builder.Services.Configure<AppSettingsDto>("TipoServidor", builder.Configuration.GetSection("TipoServidor"));
            ////builder.Services.Configure<AppSettingsDto>("Local", builder.Configuration.GetSection("OllamaLocal"));
            ////builder.Services.Configure<AppSettingsDto>("Docker", builder.Configuration.GetSection("OllamaDocker"));

            //Desabilitar totalmente o ModelState automático do .NET para permitir validação e tratamento manual
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddTransient<HelperConsoleColor>();
            builder.Services.AddHttpClient<OllamaServico>();

            builder.Services.AddScoped<IOllamaServico, OllamaServico>();

            //builder.Services.AddSingleton<EngenhariaPromptDocumentos>(sp =>
            //{
            //    var docs = new List<DocumentoContextoDto>
            //    {
            //        new DocumentoContextoDto("1", "Batman", "É um super-herói da DC conhecido como o Cavaleiro das Trevas, que protege Gotham City."),
            //        new DocumentoContextoDto("2", "Superman", "É um super-herói da DC vindo de Krypton, com poderes como superforça, visão de calor e voo."),
            //        new DocumentoContextoDto("3", "Mulher-Maravilha", "É uma amazona guerreira da DC, com força sobre-humana e o Laço da Verdade."),
            //        new DocumentoContextoDto("4", "Flash", "É o velocista escarlate da DC, capaz de correr em velocidades incríveis e manipular o tempo."),
            //        new DocumentoContextoDto("5", "Aquaman", "É o rei de Atlântida na DC, com poderes de controlar o mar e se comunicar com criaturas marinhas."),
            //    };

            //    return new EngenhariaPromptDocumentos(docs);
            //});
            builder.Services.AddSingleton<SessaoMemoriaDto>();
            builder.Services.AddSingleton<EngenhariaPromptBase>();
            builder.Services.AddSingleton<EngenhariaPromptDadosMocados>();
            builder.Services.AddScoped<ISessaoCommandRepositorio, SessaoCommandRepositorio>();
            builder.Services.AddScoped<IDocumentoCommandRepositorio, DocumentoCommandRepositorio>();


        }
    }
}
