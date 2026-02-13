using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Infrastructure.Consumers;
using DesafioMinervaFoods.Infrastructure.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace DesafioMinervaFoods.Tests.Infrastructure.Consumers
{
    public class OrderNotificationConsumerTests
    {
        private readonly Mock<IHubContext<OrderHub>> _hubContextMock;
        private readonly Mock<IHubClients> _clientsMock;
        private readonly Mock<IClientProxy> _clientProxyMock;
        private readonly OrderNotificationConsumer _consumer;

        public OrderNotificationConsumerTests()
        {
            _hubContextMock = new Mock<IHubContext<OrderHub>>();
            _clientsMock = new Mock<IHubClients>();
            _clientProxyMock = new Mock<IClientProxy>();

            // Configuração em cascata: Hub -> Clients -> All (ou Group/User)
            _hubContextMock.Setup(h => h.Clients).Returns(_clientsMock.Object);
            _clientsMock.Setup(c => c.All).Returns(_clientProxyMock.Object);

            _consumer = new OrderNotificationConsumer(_hubContextMock.Object);
        }

        [Fact]
        public async Task Deve_EnviarMensagemParaHub_Quando_EventoProcessadoForConsumido()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();
            var @event = new OrderProcessedEvent(orderId, customerId, "Sucesso", "Pedido gerado!");

            var consumeContextMock = new Mock<ConsumeContext<OrderProcessedEvent>>();
            consumeContextMock.Setup(c => c.Message).Returns(@event);

            // Act
            await _consumer.Consume(consumeContextMock.Object);

            // Assert
            // Verifica se o SendAsync foi chamado no "All" com os parâmetros corretos
            _clientProxyMock.Verify(
                p => p.SendCoreAsync(
                    "OrderNotification",
                    It.Is<object[]>(args =>
                        args.Length == 1 &&
                        args[0].ToString().Contains(orderId.ToString())),
                    default),
                Times.Once);
        }
    }
}