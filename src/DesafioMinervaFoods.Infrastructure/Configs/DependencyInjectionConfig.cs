using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class DependencyInjection
    {
        
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {            
            // assembly path
            var applicationAssembly = typeof(Result).Assembly;

            // Registrar o MediatR usando o assembly da Application
            services.AddMediatR(cfg =>
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
    }
}
