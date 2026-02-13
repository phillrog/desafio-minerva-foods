using DesafioMinervaFoods.Application.Features.Orders.Commands.ApproveOrder;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Domain.Enums;
using FluentAssertions;
using Moq;

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
        public async Task Deve_RetornarSucesso_Quando_PedidoExistir_E_For_Aprovado()
        {
            // Arrange
            var orderId = Guid.NewGuid();            
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>());

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            var command = new ApproveOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            // order.Aprovar() realmente mudou o estado no domínio
            order.Status.Should().Be(StatusEnum.Pago);
            _repositoryMock.Verify(r => r.UpdateAsync(order), Times.Once);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_Pedido_Nao_For_Encontrado()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync((Order)null!);

            var command = new ApproveOrderCommand(orderId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();

            result.Errors.Should().Contain("Pedido não encontrado ou não requer aprovação.");

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Garantir_Que_O_Metodo_Update_Seja_Chamado_Com_O_Objeto_Correto()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>());
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

            var command = new ApproveOrderCommand(orderId);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            // Verifica se o objeto passado para o Update é exatamente a instância que recuperou e alterou
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Id == order.Id)), Times.Once);
        }
    }
}