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

            var customers = new List<Customer>
            {
                new ("Avaliador Minerva Foods Brasil", "avaliador@cliente.com.br"),
                new ("Sara", "sara@cliente.com.br"),
                new ("Phillipe Roger Souza", "psouza@cliente.com.br")
            };                

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
                    await context.Customers.AddRangeAsync(customers);
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
            // Role
            const string adminRole = "Admin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            // usuários para Seed
            var seedUsers = new List<(string Email, string Password)>
            {
                ("avaliador@minervafoods.com.br", "1234"),
                ("psouza@minervafoods.com.br", "1234")
            };

            foreach (var userData in seedUsers)
            {
                var user = await userManager.FindByEmailAsync(userData.Email);

                // Se o usuário não existe, cria
                if (user == null)
                {
                    user = new IdentityUser
                    {
                        UserName = userData.Email,
                        Email = userData.Email,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(user, userData.Password);

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new Exception($"Erro ao criar usuário {userData.Email}: {errors}");
                    }
                }

                // Verifica se o usuário já tem a role para evitar o conflito de duplicidade ou FK
                if (!await userManager.IsInRoleAsync(user, adminRole))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, adminRole);
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        throw new Exception($"Erro ao atribuir role para {userData.Email}: {errors}");
                    }
                }
            }
        }
    }
}