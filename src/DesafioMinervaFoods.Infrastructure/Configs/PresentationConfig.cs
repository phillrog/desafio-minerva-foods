using Microsoft.Extensions.DependencyInjection;

namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class PresentationConfig
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddSignalR(opt => opt.EnableDetailedErrors = true);

            services.AddCors(options => {
                options.AddPolicy("Cors", policy => {
                    policy.SetIsOriginAllowed(origin => true)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            return services;
        }
    }
}
