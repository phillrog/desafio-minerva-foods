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
using System.Reflection;

namespace DesafioMinervaFoods.Tests.Infrastructure.Consumers
{
    public class ApproveOrderConsumerTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IPublishEndpoint> _publishMock;
        private readonly ApproveOrderConsumer _consumer;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public ApproveOrderConsumerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            // Configuração do Banco em Memória
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options, _currentUserServiceMock.Object);
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
            var userId = Guid.NewGuid(); // ID do usuário que está aprovando

            var order = new Order(customerId, Guid.NewGuid(), new List<OrderItem>());

            // IMPORTANTE: Garantir que o ID da entidade seja o mesmo que o comando vai buscar
            SetPrivateProperty(order, nameof(Order.Id), orderId);
            SetPrivateProperty(order, nameof(Order.RequiresManualApproval), true);

            // Criamos o comando com o ID correto
            var command = new ProcessOrderApprovalCommand(orderId, userId);

            // Mock do contexto do MassTransit
            var consumeContextMock = new Mock<ConsumeContext<ProcessOrderApprovalCommand>>();
            consumeContextMock.Setup(c => c.Message).Returns(command);

            // Mock do Repositório: Quando buscar o orderId (com qualquer userId), retorna o pedido
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<Guid?>()))
                           .ReturnsAsync(order);

            // Act
            await _consumer.Consume(consumeContextMock.Object);

            // Assert
            order.RequiresManualApproval.Should().BeFalse("O consumer deveria ter alterado para false");

            // Verifica se o Update foi chamado para o pedido correto
            _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Order>(o => o.Id == orderId)), Times.Once);

            // Verifica se o evento de sucesso foi publicado
            _publishMock.Verify(p => p.Publish(
                It.Is<OrderProcessedEvent>(e => e.Id == orderId && e.Title == "Sucesso"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Ignorar_Quando_PedidoNaoExistir()
        {
            // Arrange
            var command = new ProcessOrderApprovalCommand(Guid.NewGuid(), It.IsAny<Guid>());
            var consumeContextMock = new Mock<ConsumeContext<ProcessOrderApprovalCommand>>();
            consumeContextMock.Setup(c => c.Message).Returns(command);

            _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync((Order)null);

            // Act
            await _consumer.Consume(consumeContextMock.Object);

            // Assert
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Order>()), Times.Never);
            _publishMock.Verify(p => p.Publish(It.IsAny<OrderProcessedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
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