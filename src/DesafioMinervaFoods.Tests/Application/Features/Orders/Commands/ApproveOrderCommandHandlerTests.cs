using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Features.Orders.Commands.ApproveOrder;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Enums;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace DesafioMinervaFoods.Tests.Application.Features.Orders.Commands
{
    public class ApproveOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly ApproveOrderCommandHandler _handler;

        public ApproveOrderCommandHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _handler = new ApproveOrderCommandHandler(_repositoryMock.Object);
        }

        [Fact]
        public async Task Deve_AprovarPedido_Quando_PedidoExisteERequerAprovacao()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            // Simulando um pedido que requer aprovação manual
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>());
            // Forçamos o estado que a regra exige (ajuste conforme seu construtor/método)
            typeof(Order).GetProperty(nameof(Order.RequiresManualApproval))?.SetValue(order, true);

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

            var command = new ApproveOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Status == StatusEnum.Pago)), Times.Once);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_PedidoNaoExistir()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order)null);
            var command = new ApproveOrderCommand(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Pedido não encontrado ou não requer aprovação.");
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_PedidoNaoRequererAprovacaoManual()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>());
            // Simula pedido que NÃO precisa de aprovação
            typeof(Order).GetProperty(nameof(Order.RequiresManualApproval))?.SetValue(order, false);

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
            var command = new ApproveOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Este pedido não requer aprovação manual.");
        }
    }
}