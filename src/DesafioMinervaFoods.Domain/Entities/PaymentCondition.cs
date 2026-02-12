
namespace DesafioMinervaFoods.Domain.Entities
{
    public class PaymentCondition
    {
        public Guid PaymentConditionId { get; private set; }
        public string Description { get; private set; }
        public int NumberOfInstallments { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public PaymentCondition() {}
        public PaymentCondition(string description, int installments)
        {
            PaymentConditionId = Guid.NewGuid();
            Description = description;
            NumberOfInstallments = installments;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
