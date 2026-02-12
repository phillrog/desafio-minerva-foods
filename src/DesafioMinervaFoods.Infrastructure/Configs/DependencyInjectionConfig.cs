using DesafioMinervaFoods.Application.Interfaces;
using DesafioMinervaFoods.Application.Services;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IOrderRepository, OrderRepository>();

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
                    var context = services.GetRequiredService<AppDbContext>();

                    // Aplica migrations pendentes
                    await context.Database.MigrateAsync();

                    // Popula os dados iniciais
                    await DbInitializer.SeedAsync(context);
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
