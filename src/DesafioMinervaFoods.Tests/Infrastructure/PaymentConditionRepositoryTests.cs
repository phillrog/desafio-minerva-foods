using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DesafioMinervaFoods.Tests.Infrastructure.Repositories
{
    public class PaymentConditionRepositoryTests
    {
        private AppDbContext CriarContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task Deve_RetornarTrue_Quando_CondicaoPagamentoExistirNoBanco()
        {
            // Arrange
            var context = CriarContextoInMemory();
            var condicao = new PaymentCondition("30 Dias", 30);
            var paymentIdGerado = condicao.PaymentConditionId;

            context.Set<PaymentCondition>().Add(condicao);
            await context.SaveChangesAsync();

            var repository = new PaymentConditionRepository(context);

            // Act
            var existe = await repository.ExistsAsync(paymentIdGerado);

            // Assert
            existe.Should().BeTrue();
        }

        [Fact]
        public async Task Deve_RetornarFalse_Quando_CondicaoPagamentoNaoExistirNoBanco()
        {
            // Arrange
            var context = CriarContextoInMemory();
            var repository = new PaymentConditionRepository(context);
            var idInexistente = Guid.NewGuid();

            // Act
            var existe = await repository.ExistsAsync(idInexistente);

            // Assert
            existe.Should().BeFalse();
        }
    }
}