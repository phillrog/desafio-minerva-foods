using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DesafioMinervaFoods.Infrastructure.Persistence.Repositories
{
    public class PaymentConditionRepository : IPaymentConditionRepository
    {
        private readonly AppDbContext _context;
        public PaymentConditionRepository(AppDbContext context) => _context = context;

        public async Task<bool> ExistsAsync(Guid id)
            => await _context.Set<PaymentCondition>()
                             .AnyAsync(p => p.PaymentConditionId == id);
    }
}
