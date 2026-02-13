using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Infrastructure.Consumers;
using DesafioMinervaFoods.Infrastructure.Persistence;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DesafioMinervaFoods.Tests.Infrastructure.Consumers
{
    public class OrderCreatedConsumerTests
    {
        private readonly AppDbContext _context;
        private readonly OrderCreatedConsumer _consumer;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        
        public OrderCreatedConsumerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            // Configura um banco em memória para não precisar de um SQL Server real nos testes
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options, _currentUserServiceMock.Object);
            _consumer = new OrderCreatedConsumer(_context);
        }

        [Fact]
        public async Task Deve_GravarPrazoDeEntrega_Quando_MensagemForConsumida()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderDate = DateTime.Now;
            var @event = new OrderCreatedEvent(orderId, orderDate);

            // Mock do contexto do MassTransit que carrega a mensagem
            var consumeContextMock = new Mock<ConsumeContext<OrderCreatedEvent>>();
            consumeContextMock.Setup(c => c.Message).Returns(@event);

            // Act
            await _consumer.Consume(consumeContextMock.Object);

            // Assert
            var deliveryTerm = await _context.DeliveryTerms
                .FirstOrDefaultAsync(d => d.OrderId == orderId);

            deliveryTerm.Should().NotBeNull();
            deliveryTerm!.DeliveryDays.Should().Be(10); // Valida a regra de negócio dos 10 dias
            deliveryTerm.OrderId.Should().Be(orderId);
        }
    }
}