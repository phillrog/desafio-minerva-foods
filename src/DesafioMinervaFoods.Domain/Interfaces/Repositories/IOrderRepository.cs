using DesafioMinervaFoods.Domain.Entities;

namespace DesafioMinervaFoods.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order?> GetByIdAsync(Guid id, Guid? userId = null);
        Task<IEnumerable<Order>> GetAllAsync(Guid? userId = null);
        Task UpdateAsync(Order order);
    }
}
