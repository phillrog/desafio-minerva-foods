namespace DesafioMinervaFoods.Domain.Entities
{
    public class DeliveryTerm : Entity<Guid>
    {
        public Guid OrderId { get; private set; }
        public DateTime EstimatedDeliveryDate { get; private set; }
        public int DeliveryDays { get; private set; }

        public DeliveryTerm() {}
        public DeliveryTerm(Guid orderId, int deliveryDays, DateTime orderDate)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;
            DeliveryDays = deliveryDays;
            EstimatedDeliveryDate = orderDate.AddDays(deliveryDays);
        }

        public void UpdateDeliverySchedule(int newDeliveryDays, DateTime orderDate)
        {
            DeliveryDays = newDeliveryDays;
            EstimatedDeliveryDate = orderDate.AddDays(newDeliveryDays);
        }
    }
}
