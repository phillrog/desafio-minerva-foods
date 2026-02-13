using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DesafioMinervaFoods.Infrastructure.Persistence
{
    public class AuthDbContext : IdentityDbContext<IdentityUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdentityUser>(b => b.ToTable("Users", "Security"));
            modelBuilder.Entity<IdentityRole>(b => b.ToTable("Roles", "Security"));
            modelBuilder.HasDefaultSchema("Security");
        }
    }
}
