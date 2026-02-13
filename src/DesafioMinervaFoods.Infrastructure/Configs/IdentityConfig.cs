using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentitySetup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddAuthJWTConfiguration(configuration);
            return services;
        }

        public static async Task UseDbInitializationAsync(this IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    // Pega os dois contextos
                    var appContext = services.GetRequiredService<AppDbContext>();
                    var authContext = services.GetRequiredService<AuthDbContext>();

                    // Aplica migrations de ambos
                    await appContext.Database.MigrateAsync();
                    await authContext.Database.MigrateAsync();

                    // Seed de Negócio
                    await DbInitializer.SeedAsync(appContext);

                    // Seed de Identidade (Passa os Managers, não o Contexto)
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await DbInitializer.SeedIdentityAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("Seed");
                    logger.LogError(ex, "Ocorreu um erro ao inicializar o Banco de Dados.");
                }
            }
        }
    }
}
