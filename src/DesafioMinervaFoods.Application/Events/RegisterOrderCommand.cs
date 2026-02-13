using DesafioMinervaFoods.Application.DTOs;

namespace DesafioMinervaFoods.Application.Events
{
    public record RegisterOrderCommand(
        Guid CustomerId,
        Guid PaymentConditionId,
        List<OrderItemRequest> Items);
}
