namespace DesafioMinervaFoods.Domain.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsAsync(Guid id);
    }
}
