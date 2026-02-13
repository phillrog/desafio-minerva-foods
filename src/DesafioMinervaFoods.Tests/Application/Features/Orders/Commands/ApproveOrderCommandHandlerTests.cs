using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Application.Features.Orders.Commands.ApproveOrder;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DesafioMinervaFoods.Tests.Application.Features.Orders.Commands
{
    public class ApproveOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IEventBus> _eventBusMock;
        private readonly ApproveOrderCommandHandler _handler;

        public ApproveOrderCommandHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _eventBusMock = new Mock<IEventBus>();
            _handler = new ApproveOrderCommandHandler(_repositoryMock.Object, _eventBusMock.Object);
        }

        [Fact]
        public async Task Deve_PublicarComandoDeAprovacao_Quando_PedidoExisteERequerAprovacao()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>());

            // Simula o estado necessário para passar na regra de negócio
            typeof(Order).GetProperty(nameof(Order.RequiresManualApproval))?.SetValue(order, true);

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

            var command = new ApproveOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Message.Should().Be("Solicitação de aprovação enviada com sucesso!");

            // VERIFICAÇÃO CHAVE: Garante que o comando foi para a fila
            _eventBusMock.Verify(b => b.PublishAsync(
                It.Is<ProcessOrderApprovalCommand>(c => c.OrderId == orderId),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // GARANTIA: O Handler NÃO deve atualizar o banco diretamente agora
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
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
            result.Errors.Should().Contain(e => e.Contains("não encontrado"));

            // Não deve publicar nada na fila
            _eventBusMock.Verify(b => b.PublishAsync(It.IsAny<ProcessOrderApprovalCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_PedidoNaoRequererAprovacaoManual()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>());

            // Simula pedido que NÃO precisa de aprovação (ou já foi aprovado)
            typeof(Order).GetProperty(nameof(Order.RequiresManualApproval))?.SetValue(order, false);

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
            var command = new ApproveOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("não requer aprovação manual"));

            // Não deve publicar nada na fila
            _eventBusMock.Verify(b => b.PublishAsync(It.IsAny<ProcessOrderApprovalCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}