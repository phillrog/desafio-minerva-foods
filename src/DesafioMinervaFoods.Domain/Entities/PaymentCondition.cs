
namespace DesafioMinervaFoods.Domain.Entities
{
    public class PaymentCondition : Entity<Guid>
    {
        public string Description { get; private set; }
        public int NumberOfInstallments { get; private set; }

        protected PaymentCondition() { }

        public PaymentCondition(string description, int installments)
        {
            Id = Guid.NewGuid();
            Description = description;
            NumberOfInstallments = installments;
        }
    }
}
