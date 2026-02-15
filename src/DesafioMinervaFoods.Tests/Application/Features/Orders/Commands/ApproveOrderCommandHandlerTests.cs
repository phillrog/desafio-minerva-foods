using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Application.Features.Orders.Commands.ApproveOrder;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using System.Reflection;

namespace DesafioMinervaFoods.Tests.Application.Features.Orders.Commands
{
    public class ApproveOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IEventBus> _eventBusMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ApproveOrderCommandHandler _handler;

        public ApproveOrderCommandHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _eventBusMock = new Mock<IEventBus>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new ApproveOrderCommandHandler(
                _repositoryMock.Object,
                _eventBusMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Deve_PublicarComandoDeAprovacao_Quando_PedidoExisteERequerAprovacao()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order(userId, Guid.NewGuid(), new List<OrderItem>());

            // Configura o Mock do serviço de usuário atual
            _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);

            // Ajusta o estado interno do pedido via Reflection
            SetPrivateProperty(order, nameof(Order.CreatedBy), userId);
            SetPrivateProperty(order, nameof(Order.RequiresManualApproval), true);

            // Mock do repositório esperando o ID do pedido e o ID do usuário logado
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId, userId)).ReturnsAsync(order);

            var command = new ApproveOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Message.Should().Be("Solicitação de aprovação enviada com sucesso!");

            // VERIFICAÇÃO: Garante que a mensagem foi para a fila
            _eventBusMock.Verify(b => b.PublishAsync(
                It.Is<ProcessOrderApprovalCommand>(c => c.OrderId == orderId),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // GARANTIA: O Handler não deve atualizar o banco diretamente (o worker fará isso)
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_PedidoNaoExistir()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);

            // Retorna null simulando que o pedido não existe ou não pertence ao usuário
            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), userId)).ReturnsAsync((Order)null!);

            var command = new ApproveOrderCommand(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("não encontrado"));

            // Não deve publicar nada na fila
            _eventBusMock.Verify(b => b.PublishAsync(
                It.IsAny<ProcessOrderApprovalCommand>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_PedidoNaoRequererAprovacaoManual()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order(userId, Guid.NewGuid(), new List<OrderItem>());

            _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);

            // Simula pedido que NÃO precisa de aprovação (ex: valor baixo ou já processado)
            SetPrivateProperty(order, nameof(Order.RequiresManualApproval), false);

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId, userId)).ReturnsAsync(order);

            var command = new ApproveOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("não requer aprovação manual"));

            // Não deve publicar na fila
            _eventBusMock.Verify(b => b.PublishAsync(
                It.IsAny<ProcessOrderApprovalCommand>(),
                It.IsAny<CancellationToken>()),
                Times.Never);
        }

        /// <summary>
        /// Helper para setar propriedades privadas/somente leitura durante os testes
        /// </summary>
        private static void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            prop?.SetValue(obj, value);
        }
    }
}