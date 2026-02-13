using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Infrastructure.Consumers;
using DesafioMinervaFoods.Infrastructure.Persistence;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DesafioMinervaFoods.Tests.Infrastructure.Consumers
{
    public class ApproveOrderConsumerTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IPublishEndpoint> _publishMock;
        private readonly ApproveOrderConsumer _consumer;
        private readonly Mock<ICurrentUserService> _icurrentUserService;

        public ApproveOrderConsumerTests()
        {
            _icurrentUserService = new Mock<ICurrentUserService>();
            // Configuração do Banco em Memória
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options, _icurrentUserService.Object);
            _repositoryMock = new Mock<IOrderRepository>();
            _publishMock = new Mock<IPublishEndpoint>();

            _consumer = new ApproveOrderConsumer(
                _context,
                _repositoryMock.Object,
                _publishMock.Object);
        }

        [Fact]
        public async Task Deve_AprovarPedidoEPublicarEvento_Quando_ComandoForValido()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var order = new Order(customerId, Guid.NewGuid(), new List<OrderItem>());

            // Garantimos que o pedido precisa de aprovação manual
            typeof(Order).GetProperty(nameof(Order.RequiresManualApproval))?.SetValue(order, true);

            var command = new ProcessOrderApprovalCommand(orderId);

            // Mock do contexto do MassTransit
            var consumeContextMock = new Mock<ConsumeContext<ProcessOrderApprovalCommand>>();
            consumeContextMock.Setup(c => c.Message).Returns(command);

            _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            await _consumer.Consume(consumeContextMock.Object);

            // Assert
            order.RequiresManualApproval.Should().BeFalse();

            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Id == order.Id)), Times.Once);
            _publishMock.Verify(p => p.Publish(
                It.Is<OrderProcessedEvent>(e => e.Id == order.Id && e.Title == "Sucesso"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Ignorar_Quando_PedidoNaoExistir()
        {
            // Arrange
            var command = new ProcessOrderApprovalCommand(Guid.NewGuid());
            var consumeContextMock = new Mock<ConsumeContext<ProcessOrderApprovalCommand>>();
            consumeContextMock.Setup(c => c.Message).Returns(command);

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Order)null);

            // Act
            await _consumer.Consume(consumeContextMock.Object);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
            _publishMock.Verify(p => p.Publish(It.IsAny<OrderProcessedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}