using DesafioMinervaFoods.Domain.Enums;

namespace DesafioMinervaFoods.Application.DTOs
{
    public record OrderResponse(
    Guid OrderId,
    decimal TotalAmount,
    StatusEnum Status,
    bool RequiresManualApproval,
    DateTime? EstimatedDeliveryDate = null,
    int? DeliveryDays = null);
}
