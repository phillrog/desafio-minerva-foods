using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<Result<OrderResponse>>;
}
