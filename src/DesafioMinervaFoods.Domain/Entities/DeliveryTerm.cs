namespace DesafioMinervaFoods.Domain.Entities
{
    public class DeliveryTerm
    {
        public Guid DeliveryTermId { get; private set; }
        public Guid OrderId { get; private set; }
        public DateTime EstimatedDeliveryDate { get; private set; }
        public int DeliveryDays { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public DeliveryTerm() {}
        public DeliveryTerm(Guid orderId, int deliveryDays, DateTime orderDate)
        {
            DeliveryTermId = Guid.NewGuid();
            OrderId = orderId;
            DeliveryDays = deliveryDays;
            EstimatedDeliveryDate = orderDate.AddDays(deliveryDays);
            CreatedAt = DateTime.UtcNow;
        }
    }
}
