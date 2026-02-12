using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Enums;
using FluentAssertions;

namespace DesafioMinervaFoods.Tests.Domain
{
    public class OrderTests
    {
        [Fact]
        public void Dado_PedidoComValorMaiorQue5000_Deve_ExigirAprovacaoManualESetadoComoCriado()
        {
            // Arrange
            var itens = new List<OrderItem>
            {
                new OrderItem("Produto Premium", 1, 5001.00m)
            };

            // Act
            var pedido = new Order(Guid.NewGuid(), Guid.NewGuid(), itens);

            // Assert
            pedido.TotalAmount.Should().Be(5001.00m);
            pedido.RequiresManualApproval.Should().BeTrue();
            pedido.Status.Should().Be(StatusEnum.Criado);
        }

        [Fact]
        public void Dado_PedidoComValorMenorOuIgualA5000_Deve_EstarPagoENaoExigirAprovacao()
        {
            // Arrange
            var itens = new List<OrderItem>
            {
                new OrderItem("Produto Padrão", 1, 5000.00m)
            };

            // Act
            var pedido = new Order(Guid.NewGuid(), Guid.NewGuid(), itens);

            // Assert
            pedido.TotalAmount.Should().Be(5000.00m);
            pedido.RequiresManualApproval.Should().BeFalse();
            pedido.Status.Should().Be(StatusEnum.Pago);
        }

        [Fact]
        public void Dado_PedidoQueRequerAprovacao_Quando_Aprovar_Deve_AlterarStatusParaPago()
        {
            // Arrange
            var itens = new List<OrderItem> { new OrderItem("Item", 1, 6000.00m) };
            var pedido = new Order(Guid.NewGuid(), Guid.NewGuid(), itens);

            // Act
            pedido.Aprovar();

            // Assert
            pedido.Status.Should().Be(StatusEnum.Pago);
            pedido.RequiresManualApproval.Should().BeFalse();
        }

        [Fact]
        public void Dado_PedidoQualquer_Quando_Cancelar_Deve_AlterarStatusParaCancelado()
        {
            // Arrange
            var itens = new List<OrderItem> { new OrderItem("Item", 1, 100.00m) };
            var pedido = new Order(Guid.NewGuid(), Guid.NewGuid(), itens);

            // Act
            pedido.Cancelar();

            // Assert
            pedido.Status.Should().Be(StatusEnum.Cancelado);
        }
    }
}
}
