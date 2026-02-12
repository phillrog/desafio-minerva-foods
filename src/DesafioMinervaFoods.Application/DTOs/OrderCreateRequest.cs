namespace DesafioMinervaFoods.Application.DTOs
{
    public record OrderCreateRequest(
    Guid CustomerId,
    Guid PaymentConditionId,
    List<OrderItemRequest> Items);
}
