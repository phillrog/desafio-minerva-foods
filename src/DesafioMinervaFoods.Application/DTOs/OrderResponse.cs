namespace DesafioMinervaFoods.Application.DTOs
{
    public record OrderResponse(
    Guid OrderId,
    decimal TotalAmount,
    string Status,
    bool RequiresManualApproval);
}
