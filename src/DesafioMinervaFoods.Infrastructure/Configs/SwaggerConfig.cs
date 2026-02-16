using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Xml.XPath;

namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddCustomizedSwagger(this IServiceCollection services, IConfiguration configuration, params Type[] assemblyAnchorTypes)
        {
            services.AddSwaggerGen(c =>
            {
                // 1. Configuração de Info, Contact e License
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Desafio Minerva Foods - API",
                    Version = "1.0",
                    Description = "API de Gestão de Pedidos v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Github",
                        Email = "contato@phillrog.com",
                        Url = new Uri("https://github.com/phillrog/desafio-minerva-foods")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var serverUrl = configuration.GetValue<string>("SWAGGER:SWAGGER_SERVER_URL");
                var ambiente = configuration.GetValue<string>("SWAGGER:SWAGGER_AMBIENTE");
                // 2. Configuração de Servers
                c.AddServer(new OpenApiServer
                {
                    Url = serverUrl,
                    Description = ambiente
                });


                var assembliesToDocument = assemblyAnchorTypes
                    .Select(t => t.Assembly)
                    .Distinct()
                    .ToList();

                if (!assembliesToDocument.Contains(Assembly.GetExecutingAssembly()))
                {
                    assembliesToDocument.Add(Assembly.GetExecutingAssembly());
                }

                foreach (var assembly in assembliesToDocument)
                {
                    var xmlFile = $"{assembly.GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                    if (File.Exists(xmlPath))
                    {
                        using var fileStream = new FileStream(xmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        using var streamReader = new StreamReader(fileStream, Encoding.UTF8);
                        c.IncludeXmlComments(() => new XPathDocument(streamReader), true);
                    }
                }

                // Configuração de Segurança JWT (Mantido)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Exemplo: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            }
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }

        /// <summary>
        /// Ativa o middleware do Swagger no pipeline do Desafio Minerva Foods.
        /// </summary>
        public static IApplicationBuilder UseCustomizedSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger"; // Acessível em /swagger
                c.DefaultModelsExpandDepth(-1); // Oculta a seção de Schemas por padrão para uma UI mais limpa
            });

            return app;
        }
    }
}
