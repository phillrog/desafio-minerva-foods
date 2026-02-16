using Microsoft.Extensions.DependencyInjection;

namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class PresentationConfig
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddSignalR(opt => opt.EnableDetailedErrors = true);

            services.AddCors(options => {
                // Permite tudo
                options.AddPolicy("Cors", policy => {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
