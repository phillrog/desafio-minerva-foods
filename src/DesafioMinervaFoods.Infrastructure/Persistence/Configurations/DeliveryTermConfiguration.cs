using DesafioMinervaFoods.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioMinervaFoods.Infrastructure.Persistence.Configurations
{
    public class DeliveryTermConfiguration : IEntityTypeConfiguration<DeliveryTerm>
    {
        public void Configure(EntityTypeBuilder<DeliveryTerm> builder)
        {
            builder.ToTable("DeliveryTerms");
            builder.HasKey(d => d.DeliveryTermId);
            builder.Property(d => d.OrderId).IsRequired();
            builder.Property(d => d.DeliveryDays).IsRequired();
        }
    }
}
