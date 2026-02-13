using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Behaviors;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Infrastructure.Consumers;
using DesafioMinervaFoods.Infrastructure.Messaging;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using DesafioMinervaFoods.Infrastructure.Services;
using FluentValidation;
using MassTransit;
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
            // Pegar informações do token da requisição
            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

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

        public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitSettings = configuration.GetSection("RabbitMq");

            services.AddMassTransit(x =>
            {
                // consumidor
                x.AddConsumer<OrderCreatedConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitSettings["Host"], h =>
                    {
                        h.Username(rabbitSettings["Username"]);
                        h.Password(rabbitSettings["Password"]);
                    });

                    // Configuração de Resiliência com Polly (Retry) embutido no MassTransit
                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                    // Apenas para não ter que construir o outbox pattern
                    cfg.UseInMemoryOutbox();                    

                    cfg.ReceiveEndpoint("order-created-queue", e =>
                    {
                        // Define o nome da Exchange de Dead Letter
                        // O RabbitMQ usará essa exchange para rotear mensagens que falharem
                        e.BindDeadLetterQueue("order-created-queue-dead-letter-exchange", "order-created-queue-poison-messages", cb => {     
                            // Aqui você pode definir regras extras, como expiração
                            cb.Durable = true;
                        });

                        // 1. POLÍTICA DE RETRY (Resiliência)
                        // Se o consumer lançar exceção, o MassTransit fará o NACK e tentará novamente 3 vezes.
                        e.UseMessageRetry(r =>
                        {
                            r.Interval(3, TimeSpan.FromSeconds(5));
                        });

                        // 2. CIRCUIT BREAKER (Opcional, mas Senior)
                        // Se a fila falhar consecutivamente por muito tempo, o Circuit Breaker "abre"
                        // para não sobrecarregar o banco de dados.
                        e.UseCircuitBreaker(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                            cb.TripThreshold = 15; // 15% de falhas
                            cb.ActiveThreshold = 10; // Mínimo de 10 mensagens
                            cb.ResetInterval = TimeSpan.FromMinutes(5);
                        });
                        e.ConfigureConsumer<OrderCreatedConsumer>(context);
                    });
                });
            });

            services.AddScoped<IEventBus, MassTransitEventBus>();

            return services;
        }
    }
}
