using DesafioMinervaFoods.Domain.Entities;

namespace DesafioMinervaFoods.Domain.Interfaces.Repositories
{
    public interface IPaymentConditionRepository
    {
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<PaymentCondition>> GetAllAsync();
    }
}
