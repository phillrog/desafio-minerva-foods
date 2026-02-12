using DesafioMinervaFoods.Domain.Enums;

namespace DesafioMinervaFoods.Application.DTOs
{
    public record OrderResponse(
    Guid OrderId,
    decimal TotalAmount,
    StatusEnum Status,
    bool RequiresManualApproval);
}
