using DesafioMinervaFoods.Domain.Entities;
using Microsoft.AspNetCore.Identity;
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

        public static async Task SeedIdentityAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Garante a Role
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminEmail = "avaliador@minerva.com.br";
            var user = await userManager.FindByEmailAsync(adminEmail);

            // Se o usuário não existe, cria
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "1234");

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Erro ao criar usuário: {errors}");
                }
            }

            // Verifica se o usuário já tem a role para evitar o conflito de duplicidade ou FK
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                var roleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Erro ao atribuir role: {errors}");
                }
            }
        }
    }
}