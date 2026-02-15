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
using System.Reflection;
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
            var userId = Guid.NewGuid(); // ID do usuário que está registrando
            var paymentId = Guid.NewGuid();

            // Comando com dados concretos
            var command = new RegisterOrderCommand(customerId, paymentId, new List<OrderItemRequest>(), userId);

            // Entidade fake que o Mapper retornará
            var orderFake = new Order(customerId, paymentId, new List<OrderItem>());

            // Garante que a entidade comece com um status diferente para validar a alteração
            SetPrivateProperty(orderFake, nameof(Order.Status), StatusEnum.Processando);

            // Simula o mapeamento
            _mapperMock.Setup(m => m.Map<Order>(It.IsAny<RegisterOrderCommand>()))
                       .Returns(orderFake);

            // Mock do MassTransit ConsumeContext
            var consumeContextMock = new Mock<ConsumeContext<RegisterOrderCommand>>();
            consumeContextMock.Setup(c => c.Message).Returns(command);

            // Act
            await _consumer.Consume(consumeContextMock.Object);

            // Assert

            // 1. Verificou se a lógica de negócio alterou o status para Processando
            orderFake.Status.Should().Be(StatusEnum.Pago);

            // 2. Verificou se persistiu via repositório o objeto mapeado
            _repositoryMock.Verify(r => r.AddAsync(It.Is<Order>(o => o.Id == orderFake.Id)), Times.Once);

            // 3. Verificou a publicação dos eventos de integração
            // Evento de criação (Geralmente dispara o início do fluxo)
            _publishMock.Verify(p => p.Publish(
                It.Is<OrderCreatedEvent>(e => e.OrderId == orderFake.Id),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // Evento de processamento (Notifica o status atual)
            _publishMock.Verify(p => p.Publish(
                It.Is<OrderProcessedEvent>(e => e.Id == orderFake.Id && e.Title == "Sucesso"),
                It.IsAny<CancellationToken>()),
                Times.Once);
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