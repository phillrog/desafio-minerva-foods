using AutoMapper;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Features.Orders.Commands.CreateOrder;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace DesafioMinervaFoods.Tests.Application.Features.Orders.Commands
{
    public class CreateOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<ICustomerRepository> _customerRepoMock;
        private readonly Mock<IPaymentConditionRepository> _paymentRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CreateOrderCommandHandler _handler;

        public CreateOrderCommandHandlerTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _customerRepoMock = new Mock<ICustomerRepository>();
            _paymentRepoMock = new Mock<IPaymentConditionRepository>();
            _mapperMock = new Mock<IMapper>();

            // repositorios
            var validator = new CreateOrderCommandValidator(_customerRepoMock.Object, _paymentRepoMock.Object);

            _handler = new CreateOrderCommandHandler(
                _orderRepoMock.Object,
                validator,
                _mapperMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarSucesso_Quando_PedidoForValido()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var paymentId = Guid.NewGuid();
            _customerRepoMock.Setup(r => r.ExistsAsync(customerId)).ReturnsAsync(true);
            _paymentRepoMock.Setup(r => r.ExistsAsync(paymentId)).ReturnsAsync(true);

            var command = new CreateOrderCommand(customerId, paymentId, new List<OrderItemRequest>
            {
                new ("Carne Maturata", 2, 150.00m)
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_Cliente_Nao_Existir_No_Banco()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest> { new("Produto", 1, 10) });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("cliente informado não existe"));
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_CondicaoPagamento_Nao_Existir_No_Banco()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _paymentRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(false);

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest> { new("Produto", 1, 10) });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("condição de pagamento informada não existe"));
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_Quantidade_Do_Item_For_Zero_Ou_Negativa()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _paymentRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest>
            {
                new ("Produto", 0, 10) // Quantidade zero
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("maior que zero"));
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_PrecoUnitario_For_Negativo()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _paymentRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest>
            {
                new ("Produto", 5, -1.50m) // Preço negativo
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("maior que zero"));
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_Nome_Do_Produto_For_Vazio()
        {
            // Arrange
            _customerRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _paymentRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Guid>())).ReturnsAsync(true);

            var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest>
            {
                new ("", 5, 10) // Nome vazio
            });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Contains("nome do produto é obrigatório"));
        }
    }
}