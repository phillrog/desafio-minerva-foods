using DesafioMinervaFoods.Domain.Enums;

namespace DesafioMinervaFoods.Application.DTOs
{
    public record OrderResponse
    {
        public Guid Id { get; init; }
        public decimal TotalAmount { get; init; }
        public StatusEnum Status { get; init; }
        public bool RequiresManualApproval { get; init; }
        public DateTime? EstimatedDeliveryDate { get; init; }
        public int? DeliveryDays { get; init; }
        public List<OrderItemResponse> Items { get; init; } = new();
    }
}
