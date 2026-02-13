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
            var pedidosFicticios = new List<Order>
            {
                new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>()),
                new Order(Guid.NewGuid(), Guid.NewGuid(), new List<OrderItem>())
            };

            // pedidos criados
            var respostaEsperada = new List<OrderResponse>
            {
                new OrderResponse(Guid.NewGuid(), 100.00m, StatusEnum.Criado, true),
                new OrderResponse(Guid.NewGuid(), 200.00m, StatusEnum.Criado, false)
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(pedidosFicticios);

            _mapperMock.Setup(m => m.Map<IEnumerable<OrderResponse>>(pedidosFicticios))
                       .Returns(respostaEsperada);

            var query = new GetAllOrdersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().HaveCount(2);
            result.Data.First().TotalAmount.Should().Be(100.00m);
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
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