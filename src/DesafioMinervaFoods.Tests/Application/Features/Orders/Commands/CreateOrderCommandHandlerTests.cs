using AutoMapper;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Application.Features.Orders.Commands.CreateOrder;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;

namespace DesafioMinervaFoods.Tests.Application.Features.Orders.Commands
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<IPaymentConditionRepository> _paymentRepoMock;
        private readonly Mock<IEventBus> _eventBusMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _paymentRepoMock = new Mock<IPaymentConditionRepository>();
            _eventBusMock = new Mock<IEventBus>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            // Instanciando o validador real com os mocks dos repositórios
            var validator = new CreateOrderCommandValidator(_customerRepoMock.Object, _paymentRepoMock.Object);

            _handler = new CreateOrderCommandHandler(
                validator,
                _eventBusMock.Object,
                _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarSucesso_EPublicarMensagem_Quando_PedidoForValido()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            
            _customerRepoMock.Setup(r => r.ExistsAsync(customerId))
                             .ReturnsAsync(true);
            _paymentRepoMock.Setup(r => r.ExistsAsync(paymentId))
                             .ReturnsAsync(true);

            var command = new CreateOrderCommand(customerId, paymentId, new List<OrderItemRequest>
            {
                new ("Carne Maturata", 2, 150.00m)
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Message.Should().Be("Pedido solicitado com sucesso!");

            // O Handler deve publicar o comando de registro no barramento
            _eventBusMock.Verify(b => b.PublishAsync(
                It.IsAny<RegisterOrderCommand>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            // O repositório de ordens NÃO deve ser chamado (quem fará isso é o Consumer da fila)
            _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_Cliente_Nao_Existir()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>()))
                             .ReturnsAsync(false);

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest> { new("Produto", 1, 10) });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("cliente informado não existe"));

            // Não deve publicar nada se a validação falhar
            _eventBusMock.Verify(b => b.PublishAsync(It.IsAny<RegisterOrderCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_Itens_Estiverem_Invalidos()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _paymentRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest>
            {
                new("", 0, -1) // Nome vazio, Qtd 0, Preço negativo
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().HaveCountGreaterThan(1);
        }
    }
}