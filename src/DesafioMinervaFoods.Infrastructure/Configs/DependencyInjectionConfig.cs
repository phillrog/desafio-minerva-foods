using DesafioMinervaFoods.Application.Interfaces;
using DesafioMinervaFoods.Application.Services;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContexts
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositórios
            services.AddScoped<IOrderRepository, OrderRepository>();

            // JWT
            services.AddAuthJWTConfiguration(configuration);

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Serviços de Aplicação
            services.AddScoped<IOrderService, OrderService>();

            // FluentValidation - Registra todos os validadores automaticamente
            // Pega todos os assemblies carregados que pertencem à sua solução
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName!.StartsWith("DesafioMinervaFoods"))
                .ToArray();

            services.AddValidatorsFromAssemblies(assemblies);

            return services;
        }
        
        public static async Task UseSeedAsync(this IHost app)
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
