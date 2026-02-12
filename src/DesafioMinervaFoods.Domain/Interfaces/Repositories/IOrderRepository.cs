using DesafioMinervaFoods.Domain.Entities;

namespace DesafioMinervaFoods.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id);
        Task<IEnumerable<Order>> GetAllAsync();
        Task UpdateAsync(Order order);
    }
}
