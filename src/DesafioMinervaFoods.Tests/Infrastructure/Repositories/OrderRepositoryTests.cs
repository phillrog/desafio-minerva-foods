using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DesafioMinervaFoods.Tests.Infrastructure
{
    public class OrderRepositoryTests
    {
        private readonly Mock<ICurrentUserService> _currentUserService;
        public OrderRepositoryTests()
        {
            _currentUserService = new Mock<ICurrentUserService>();
        }

        [Fact]
        public async Task AddAsync_Deve_PersistirPedidoNoBancoInMemory()
        {
            // Arrange
            var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Vai gravar um pedido
            using (var contextGrava = new AppDbContext(dbContextOptions, _currentUserService.Object))
            {
                var repository = new OrderRepository(contextGrava);
                var order = new Order(Guid.NewGuid(), Guid.NewGuid(),
                    new List<OrderItem> { new("Teste", 1, 100) });

                // Act
                await repository.AddAsync(order);
            }

            // Assert - lê e garante a persistência
            using (var contextLeitura = new AppDbContext(dbContextOptions, _currentUserService.Object))
            {
                var result = await contextLeitura.Orders.Include(o => o.Items)
                    .FirstOrDefaultAsync();

                result.Should().NotBeNull();
                result!.TotalAmount.Should().Be(100);
                result.Items.Should().HaveCount(1);
            }
        }
    }
}
