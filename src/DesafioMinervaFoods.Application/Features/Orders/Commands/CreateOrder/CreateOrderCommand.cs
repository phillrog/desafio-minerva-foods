using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(
        Guid CustomerId,
        Guid PaymentConditionId,
        List<OrderItemRequest> Items) : IRequest<Result<OrderResponse>>;
}
