using AutoMapper;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Enums;
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
    public class RegisterOrderConsumerTests
    {
        private readonly AppDbContext _context;
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IPublishEndpoint> _publishMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly RegisterOrderConsumer _consumer;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public RegisterOrderConsumerTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _context = new AppDbContext(options, _currentUserServiceMock.Object);
            _repositoryMock = new Mock<IOrderRepository>();
            _publishMock = new Mock<IPublishEndpoint>();
            _mapperMock = new Mock<IMapper>();

            _consumer = new RegisterOrderConsumer(
                _context,
                _repositoryMock.Object,
                _publishMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Deve_ProcessarEPublicarEventos_Quando_ComandoForRecebido()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var command = new RegisterOrderCommand(customerId, Guid.NewGuid(), new List<OrderItemRequest>(), It.IsAny<Guid>());

            var orderFake = new Order(customerId, Guid.NewGuid(), new List<OrderItem>());

            // Simula o mapeamento do comando para a entidade
            _mapperMock.Setup(m => m.Map<Order>(It.IsAny<RegisterOrderCommand>())).Returns(orderFake);

            // Mock do MassTransit ConsumeContext
            var consumeContextMock = new Mock<ConsumeContext<RegisterOrderCommand>>();
            consumeContextMock.Setup(c => c.Message).Returns(command);

            // Act
            await _consumer.Consume(consumeContextMock.Object);

            // Assert
            // 1. Verificou se definiu o status (Lógica de Domínio)
            orderFake.Status.Should().Be(StatusEnum.Processando);

            // 2. Verificou se persistiu via repositório
            _repositoryMock.Verify(r => r.AddAsync(It.Is<Order>(o => o.Id == orderFake.Id)), Times.Once);

            // 3. Verificou se publicou os dois eventos esperados
            _publishMock.Verify(p => p.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
            _publishMock.Verify(p => p.Publish(It.IsAny<OrderProcessedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}