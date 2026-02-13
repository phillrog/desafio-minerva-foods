namespace DesafioMinervaFoods.Domain.Entities
{   
    public class OrderItem : Entity<Guid>
    {
        public Guid OrderId { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        protected OrderItem() { }

        public OrderItem(string productName, int quantity, decimal unitPrice)
        {
            Id = Guid.NewGuid(); 
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }               
    }
}