using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DesafioMinervaFoods.Infrastructure.Persistence
{
    public class AppDbContext : DbContext, IUnitOfWork
    {
        private readonly ICurrentUserService _currentUserService;

        private IDbContextTransaction _currentTransaction;

        public AppDbContext(DbContextOptions<AppDbContext> options,
            ICurrentUserService currentUserService) : base(options) {
            _currentUserService = currentUserService;
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PaymentCondition> PaymentConditions { get; set; }
        public DbSet<DeliveryTerm> DeliveryTerms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // aplica todas as configurações que encontrar no projeto!
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            // Captura todas as entradas que herdam de EntityAudit (ou sua classe de base)
            var entries = ChangeTracker.Entries()
                .Where(x => (x.Entity.GetType().BaseType?.IsGenericType == true &&
                            x.Entity.GetType().BaseType?.GetGenericTypeDefinition() == typeof(Entity<>)) &&
                            (x.State == EntityState.Added ||
                            x.State == EntityState.Modified ||
                            x.State == EntityState.Deleted))
                .ToList();

            if (!entries.Any()) return;
            
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            foreach (var entidade in entries)
            {
                switch (entidade.State)
                {
                    // Criação
                    case EntityState.Added:
                        entidade.Property("CreatedAt").CurrentValue = DateTime.UtcNow; ;
                        entidade.Property("CreatedBy").CurrentValue = currentUserId;
                        break;
                    // Alteração
                    case EntityState.Modified:
                        entidade.Property("UpdatedAt").CurrentValue = false;
                        entidade.Property("UpdatedBy").CurrentValue = DateTime.UtcNow; ;
                        break;                    
                }
            }
        }

        #region [ TRANSAÇÃO ]

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _currentTransaction ??= await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                if (_currentTransaction != null)
                {
                    await _currentTransaction.CommitAsync(cancellationToken);
                }
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.RollbackAsync(cancellationToken);
                }
            }
            finally
            {
                DisposeTransaction();
            }
        }

        private void DisposeTransaction()
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null!;
            }
        }
        #endregion
    }
}
