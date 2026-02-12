using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Validations;
using FluentAssertions;

namespace DesafioMinervaFoods.Tests.Application
{
    public class OrderValidatorTests
    {
        private readonly OrderCreateValidator _validator = new();

        [Fact]
        public void Deve_RetornarErro_Quando_PedidoNaoTemItens()
        {
            var request = new OrderCreateRequest(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItemRequest>());
            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Deve_RetornarErro_Quando_ItemTemPrecoZero()
        {
            var request = new OrderCreateRequest(Guid.NewGuid(), Guid.NewGuid(),
                new List<OrderItemRequest> { new OrderItemRequest("Produto", 1, 0) });

            var result = _validator.Validate(request);
            result.IsValid.Should().BeFalse();
        }
    }
}
