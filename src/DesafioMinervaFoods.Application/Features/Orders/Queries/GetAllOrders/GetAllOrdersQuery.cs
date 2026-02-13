using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Queries.GetAllOrders
{
    public record GetAllOrdersQuery() : IRequest<Result<IEnumerable<OrderResponse>>>;
}
