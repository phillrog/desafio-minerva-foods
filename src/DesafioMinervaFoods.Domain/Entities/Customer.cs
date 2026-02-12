namespace DesafioMinervaFoods.Domain.Entities
{
    public class Customer
    {
        public Guid CustomerId { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Customer() { }
                    
        public Customer(string name, string email)
        {
            CustomerId = Guid.NewGuid();
            Name = name;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
