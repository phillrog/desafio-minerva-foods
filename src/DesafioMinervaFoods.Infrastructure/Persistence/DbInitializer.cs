using DesafioMinervaFoods.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DesafioMinervaFoods.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Se já houver clientes, não executa o seed
            if (context.Customers.Any()) return;
            
            var customer = new Customer("Avaliador Minerva Foods Brasil", "avaliador@minerva.com.br");

            var paymentConditions = new List<PaymentCondition>
            {
                new ("À Vista", 1),
                new ("30/60/90 dias", 3),
                new ("Parcelado 12x", 12)
            };

            // Implementando a estratégia de execução para resiliência
            var strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    await context.Customers.AddAsync(customer);
                    await context.PaymentConditions.AddRangeAsync(paymentConditions);

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw; // Repassa para o logger no DependencyInjection
                }
            });
        }
    }
}