using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Behaviors;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using FluentValidation;
using MediatR;
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
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPaymentConditionRepository, PaymentConditionRepository>();

            // JWT
            services.AddAuthJWTConfiguration(configuration);

            // UoW
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<AppDbContext>()!);

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Serviços de Aplicação
            
            // assembly path
            var applicationAssembly = typeof(Result).Assembly;

            // Registrar o MediatR usando o assembly da Application
            services.AddMediatR(cfg=>
            {
                cfg.RegisterServicesFromAssembly(applicationAssembly);

                // PIPELINE TRANSAÇÂO
                cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
            });

            // Registrar o AutoMapper usando o assembly da Application
            services.AddAutoMapper(applicationAssembly);

            // Validadores
            services.AddValidatorsFromAssembly(applicationAssembly);

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
