namespace DesafioMinervaFoods.Application.DTOs
{
    public record OrderItemResponse(
        Guid OrderItemId,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice);
}
