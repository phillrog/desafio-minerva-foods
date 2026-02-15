using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<Order?> GetByIdAsync(Guid id, Guid? userId = null)
        {
            var query = _context.Orders.AsNoTracking()
                .Include(o => o.Items)
                .Include(o => o.DeliveryTerm)
                .Include(o => o.Customer)
                .Include(o => o.PaymentCondition)
                .AsQueryable()
                .Where(o => o.Id == id);

            if (userId.HasValue && userId != Guid.Empty)
            {
                query = query.Where(o => o.CreatedBy == userId.Value);
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Order>> GetAllAsync(Guid? userId = null)
        {
            var query = _context.Orders.AsNoTracking()
                .Include(o => o.Items)
                .Include(o => o.DeliveryTerm)
                .Include(o => o.Customer)
                .Include(o => o.PaymentCondition)
                .AsQueryable();

            if (userId.HasValue && userId != Guid.Empty)
            {
                query = query.Where(o => o.CreatedBy == userId.Value);
            }

            return await query.ToListAsync();
        }
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
