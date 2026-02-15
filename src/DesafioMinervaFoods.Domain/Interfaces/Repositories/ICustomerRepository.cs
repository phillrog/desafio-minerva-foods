using DesafioMinervaFoods.Domain.Entities;

namespace DesafioMinervaFoods.Domain.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<bool> ExistsAsync(Guid id);
        Task<IEnumerable<Customer>> GetAllAsync();
    }
}
