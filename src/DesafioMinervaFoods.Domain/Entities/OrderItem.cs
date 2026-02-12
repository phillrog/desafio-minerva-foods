namespace DesafioMinervaFoods.Domain.Entities
{
    public class OrderItem
    {
        public Guid OrderItemId { get; private set; }
        public Guid OrderId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        public OrderItem() {}
        public OrderItem(string productName, int quantity, decimal unitPrice)
        {
            OrderItemId = Guid.NewGuid();
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
