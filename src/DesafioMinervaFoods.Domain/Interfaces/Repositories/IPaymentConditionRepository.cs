namespace DesafioMinervaFoods.Domain.Interfaces.Repositories
{
    public interface IPaymentConditionRepository
    {
        Task<bool> ExistsAsync(Guid id);
    }
}
