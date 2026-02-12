using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Services;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DesafioMinervaFoods.Tests.Application
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _service = new OrderService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_Deve_ChamarRepositoryESalvarPedido()
        {
            // Arrange
            var request = new OrderCreateRequest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<OrderItemRequest> { new("Produto A", 1, 1000) }
            );

            // Mock para aceitar qualquer objeto Order e retornar uma Task completada
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
                           .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateOrderAsync(request);

            // Assert
            // Verifica se o serviço chamou o método AddAsync da INTERFACE exatamente uma vez
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);

            result.TotalAmount.Should().Be(1000);
            result.Status.Should().Be("Pago");
        }
    }
}
