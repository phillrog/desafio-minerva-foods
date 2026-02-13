using AutoMapper;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Features.Orders.Queries.GetAllOrders;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Domain.Enums;
using FluentAssertions;
using Moq;

namespace DesafioMinervaFoods.Tests.Application.Features.Orders.Queries
{
    public class GetAllOrdersQueryHandlerTests
    {
        private readonly Mock<IOrderRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetAllOrdersQueryHandler _handler;

        public GetAllOrdersQueryHandlerTests()
        {
            _repositoryMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetAllOrdersQueryHandler(_repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Deve_Retornar_Lista_De_Pedidos_Com_Sucesso_Quando_Existirem_Dados()
        {
            // Arrange
            // Criando itens para que a soma não seja zero
            var itens = new List<OrderItem> { new OrderItem("Produto Teste", 1, 100.00m) };

            var pedidosFicticios = new List<Order>
    {
        new Order(Guid.NewGuid(), Guid.NewGuid(), itens),
        new Order(Guid.NewGuid(), Guid.NewGuid(), itens)
    };

            var respostaEsperada = new List<OrderResponse>
    {
        // Se o seu OrderResponse agora é um record com propriedades { get; init }, 
        // use a inicialização de objeto:
        new OrderResponse { Id = Guid.NewGuid(), TotalAmount = 100.00m, Status = nameof(StatusEnum.Criado) },
        new OrderResponse { Id = Guid.NewGuid(), TotalAmount = 200.00m, Status = nameof(StatusEnum.Criado) }
    };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(pedidosFicticios);

            // DICA: Use It.IsAny para evitar problemas de comparação de referência de listas
            _mapperMock.Setup(m => m.Map<IEnumerable<OrderResponse>>(It.IsAny<IEnumerable<Order>>()))
                       .Returns(respostaEsperada);

            var query = new GetAllOrdersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.First().TotalAmount.Should().Be(100.00m); // Agora deve bater!
        }

        [Fact]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Nao_Houver_Pedidos_No_Banco()
        {
            // Arrange
            var pedidosVazios = new List<Order>();
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(pedidosVazios);

            _mapperMock.Setup(m => m.Map<IEnumerable<OrderResponse>>(pedidosVazios))
                       .Returns(new List<OrderResponse>());

            var query = new GetAllOrdersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }
    }
}