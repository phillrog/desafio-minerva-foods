using AutoMapper;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Features.Orders.Queries.GetOrderById;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Domain.Enums;
using FluentAssertions;
using Moq;

namespace DesafioMinervaFoods.Tests.Application.Features.Orders.Queries
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetOrderByIdQueryHandler _handler;

        public GetOrderByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetOrderByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Pedido_Com_Sucesso_Quando_O_Id_Existir()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>());

            var responseEsperada = new OrderResponse
            {
                Id = orderId,
                TotalAmount = 150.00m,
                Status = StatusEnum.Criado,
                RequiresManualApproval = true
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _mapperMock.Setup(m => m.Map<OrderResponse>(order))
                .Returns(responseEsperada);

            var query = new GetOrderByIdQuery(orderId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(orderId);
            _repositoryMock.Verify(r => r.GetByIdAsync(orderId), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Falha_Quando_O_Pedido_Nao_For_Encontrado()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null!);

            var query = new GetOrderByIdQuery(orderId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            // Verificando se a mensagem de erro está na lista de Errors do seu Result
            result.Errors.Should().Contain("Pedido não encontrado.");
            _mapperMock.Verify(m => m.Map<OrderResponse>(It.IsAny<Order>()), Times.Never);
        }
    }
}