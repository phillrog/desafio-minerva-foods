using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.Interfaces;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DesafioMinervaFoods.Tests.Infrastructure.Repositories
{
    public class PaymentConditionRepositoryTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public PaymentConditionRepositoryTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
        }

        private AppDbContext CriarContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarTrue_Quando_CondicaoPagamentoExistirNoBanco()
        {
            // Arrange
            var context = CriarContextoInMemory();
            var condicao = new PaymentCondition("30 Dias", 30);
            var paymentIdGerado = condicao.Id;

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

        [Fact]
        public async Task Deve_RetornarTodasAsCondicoesDePagamento_Quando_ExistiremNoBanco()
        {
            // Arrange
            var context = CriarContextoInMemory();

            var condicao1 = new PaymentCondition("À Vista", 1);
            var condicao2 = new PaymentCondition("30/60 Dias", 2);

            await context.PaymentConditions.AddRangeAsync(condicao1, condicao2);
            await context.SaveChangesAsync();

            var repository = new PaymentConditionRepository(context);

            // Act
            var resultado = await repository.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.Should().Contain(p => p.Description == "À Vista");
            resultado.Should().Contain(p => p.Description == "30/60 Dias");
        }

        [Fact]
        public async Task Deve_RetornarListaVazia_Quando_NaoHouverCondicoesDePagamento()
        {
            // Arrange
            var context = CriarContextoInMemory();
            var repository = new PaymentConditionRepository(context);

            // Act
            var resultado = await repository.GetAllAsync();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }
    }
}