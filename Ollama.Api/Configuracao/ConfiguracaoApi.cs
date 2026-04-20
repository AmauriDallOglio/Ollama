using Microsoft.OpenApi.Models;
using Ollama.Aplicacao.Util;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Ollama.Api.Configuracao
{
    public static class ConfiguracaoApi
    {
        public static void ConfiguracaoSwagger(this IServiceCollection services)
        {

            PrintaConsole.Info("Carregando configuração swager");
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.ConfigurarSwagger();
            });

            services.AddHttpContextAccessor();

            PrintaConsole.Info("Carregando controllers");
            Controllers(services);
            PrintaConsole.Info("Carregando autorização");
            Autorizacao(services);
            PrintaConsole.Info("Carregando CORS");
            Cors(services);

            // Registra o IHttpClientFactory
            services.AddHttpClient();
        }

        private static void ConfigurarSwagger(this SwaggerGenOptions c)
        {
 
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Ollama API",
                Version = "v1",
                Description = "API de integração com o Ollama (Llama3.2)"
            });
       

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Informe: Bearer {seu token}",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        }



        private static void Controllers(this IServiceCollection services)
        {

           // services.AddControllers();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true; // deixa o JSON "bonito"
            }); 

        }



        private static void Cors(this IServiceCollection services)
        {

            // Adiciona CORS
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        private static void Autorizacao(this IServiceCollection services)
        {
            services.AddAuthorization();
            //AutenticacaoJwt(services);
        }



        //private static void AutenticacaoJwt(this IServiceCollection services)
        //{
        //    var provider = services.BuildServiceProvider();
        //    var configuration = provider.GetRequiredService<IConfiguration>();
        //    var publicKeyBase64 = configuration["Token:PublicKey"];
        //    var issuer = configuration["Token:Issuer"];
        //    var audience = configuration["Token:Audience"];

        //    PrintaConsole.Info($"Configurando JWT - Issuer: {issuer}, Audience: {audience}");
        //    PrintaConsole.Info($"PublicKey length: {publicKeyBase64?.Length ?? 0}");

        //    if (string.IsNullOrEmpty(publicKeyBase64))
        //    {
        //        throw new InvalidOperationException("Token:PublicKey não está configurado no appsettings.json");
        //    }

        //    try
        //    {
        //        using var rsa = RSA.Create();
        //        rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKeyBase64), out _);
        //        var securityKey = new RsaSecurityKey(rsa);

        //        services.AddAuthentication(options =>
        //        {
        //            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //        })
        //        .AddJwtBearer(options =>
        //        {
        //            options.RequireHttpsMetadata = false;
        //            options.SaveToken = true;
        //            options.TokenValidationParameters = new TokenValidationParameters
        //            {
        //                ValidateIssuerSigningKey = true,
        //                IssuerSigningKey = securityKey,
        //                ValidateIssuer = true,
        //                ValidIssuer = issuer,
        //                ValidateAudience = true,
        //                ValidAudience = audience,
        //                ValidateLifetime = true,
        //                ClockSkew = TimeSpan.Zero
        //            };

        //            options.Events = new JwtBearerEvents
        //            {
        //                OnAuthenticationFailed = context =>
        //                {
        //                    PrintaConsole.Error($"JWT Authentication Failed: {context.Exception.Message}");
        //                    return Task.CompletedTask;
        //                },
        //                OnTokenValidated = context =>
        //                {
        //                    PrintaConsole.Sucesso("JWT Token Validated Successfully");
        //                    return Task.CompletedTask;
        //                }
        //            };
        //        });

        //        PrintaConsole.Sucesso("JWT Bearer configurado com sucesso");
        //    }
        //    catch (Exception ex)
        //    {
        //        PrintaConsole.Error($"Erro ao configurar JWT: {ex.Message}");
        //        throw;
        //    }
        //}

    }
}
