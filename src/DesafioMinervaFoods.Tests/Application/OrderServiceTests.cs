using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Services;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Enums;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace DesafioMinervaFoods.Tests.Application
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IValidator<OrderCreateRequest>> _validatorMock;
        private readonly OrderService _service;

        public OrderServiceTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _validatorMock = new Mock<IValidator<OrderCreateRequest>>();
            _service = new OrderService(_repositoryMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_Deve_RetornarSucesso_QuandoPedidoEhValido()
        {
            // Arrange
            var request = new OrderCreateRequest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<OrderItemRequest> { new("Produto A", 1, 1000) }
            );

            // Mock da Validação (Sucesso)
            _validatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateOrderAsync(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.TotalAmount.Should().Be(1000);
            result.Data.Status.Should().Be(StatusEnum.Pago);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_Deve_RetornarFalha_QuandoValidacaoFalha()
        {
            // Arrange
            var request = new OrderCreateRequest(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest>());
            var validationFailures = new List<ValidationFailure> { new("Items", "O pedido deve ter itens") };

            _validatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act
            var result = await _service.CreateOrderAsync(request);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("O pedido deve ter itens");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task ApproveOrderAsync_Deve_RetornarErro_QuandoPedidoNaoExiste()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order)null!);

            // Act
            var result = await _service.ApproveOrderAsync(orderId);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Pedido não encontrado ou não requer aprovação.");
        }

        [Fact]
        public async Task CreateOrderAsync_Deve_RetornarStatusCriado_QuandoValorExceder5000()
        {
            // Arrange
            var request = new OrderCreateRequest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                new List<OrderItemRequest> { new("Máquina Industrial", 1, 6000) }
            );

            _validatorMock.Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult());

            // Act
            var result = await _service.CreateOrderAsync(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data!.Status.Should().Be(StatusEnum.Criado);
            result.Data.RequiresManualApproval.Should().BeTrue();
        }
    }
}
