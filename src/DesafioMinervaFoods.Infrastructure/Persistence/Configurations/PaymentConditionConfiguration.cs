using DesafioMinervaFoods.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioMinervaFoods.Infrastructure.Persistence.Configurations
{
    public class PaymentConditionConfiguration : IEntityTypeConfiguration<PaymentCondition>
    {
        public void Configure(EntityTypeBuilder<PaymentCondition> builder)
        {
            builder.ToTable("PaymentConditions");
            builder.HasKey(p => p.PaymentConditionId);
            builder.Property(p => p.Description).HasMaxLength(100).IsRequired();
        }
    }
}
