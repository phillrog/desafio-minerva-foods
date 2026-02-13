using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioMinervaFoods.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id) =>
            await _context.Orders.Include(o => o.Items)
                                 .Include(o => o.DeliveryTerm)
                                 .FirstOrDefaultAsync(o => o.Id == id);

        public async Task<IEnumerable<Order>> GetAllAsync() =>
            await _context.Orders.AsNoTracking()
                                 .Include(o => o.Items)
                                 .Include(o => o.DeliveryTerm)
                                 .ToListAsync();

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
