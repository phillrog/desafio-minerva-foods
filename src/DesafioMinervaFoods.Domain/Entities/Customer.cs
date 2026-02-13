namespace DesafioMinervaFoods.Domain.Entities
{
    public class Customer : Entity<Guid>
    {        
        public string Name { get; private set; }
        public string Email { get; private set; }

        public Customer() { }
                    
        public Customer(string name, string email)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
