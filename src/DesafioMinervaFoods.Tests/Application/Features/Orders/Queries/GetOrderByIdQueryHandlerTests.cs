using AutoMapper;
using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Features.Orders.Queries.GetOrderById;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Enums;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DesafioMinervaFoods.Tests.Application.Features.Orders.Queries
{
    public class GetOrderByIdQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly GetOrderByIdQueryHandler _handler;

        public GetOrderByIdQueryHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new GetOrderByIdQueryHandler(
                _repositoryMock.Object,
                _mapperMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Pedido_Com_Sucesso_Quando_O_Id_Existir_E_Pertencer_Ao_Usuario()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order(userId, Guid.NewGuid(), new List<OrderItem>());

            var responseEsperada = new OrderResponse
            {
                Id = orderId,
                TotalAmount = 150.00m,
                Status = nameof(StatusEnum.Criado),
                RequiresManualApproval = true
            };

            // Configura o usuário logado
            _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);

            // O repositório deve receber o ID do pedido e o ID do usuário logado
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId, userId))
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

            // Verificação rigorosa dos parâmetros passados ao repositório
            _repositoryMock.Verify(r => r.GetByIdAsync(orderId, userId), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_Falha_Quando_O_Pedido_Nao_For_Encontrado_Ou_Nao_Pertencer_Ao_Usuario()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);

            // Simula que o repositório não encontrou o pedido (seja por ID inexistente ou UserId divergente)
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId, userId))
                .ReturnsAsync((Order)null!);

            var query = new GetOrderByIdQuery(orderId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Pedido não encontrado.");

            // Garante que o mapeamento nem foi tentado já que o objeto é nulo
            _mapperMock.Verify(m => m.Map<OrderResponse>(It.IsAny<Order>()), Times.Never);
        }
    }
}