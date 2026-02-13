namespace DesafioMinervaFoods.Application.DTOs
{
    public record OrderItemResponse(
        Guid Id,
        string ProductName,
        int Quantity,
        decimal UnitPrice,
        decimal TotalPrice)
    {
        public OrderItemResponse() : this(default, string.Empty, default, default, default) { }
    }
}
