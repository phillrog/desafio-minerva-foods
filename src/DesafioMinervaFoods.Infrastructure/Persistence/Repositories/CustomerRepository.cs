using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioMinervaFoods.Infrastructure.Persistence.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context) => _context = context;

        public async Task<bool> ExistsAsync(Guid id)
            => await _context.Set<Customer>()
                             .AnyAsync(c => c.Id == id);
        public async Task<IEnumerable<Customer>> GetAllAsync() =>
            await _context.Customers.AsNoTracking().ToListAsync();
    }
}
