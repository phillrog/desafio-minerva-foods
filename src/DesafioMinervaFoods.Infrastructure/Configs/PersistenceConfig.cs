using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class PersistenceConfig
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Pegar informações do token da requisição
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IPaymentConditionRepository, PaymentConditionRepository>();

            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<AppDbContext>());

            return services;
        }
    }
}
