using DesafioMinervaFoods.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DesafioMinervaFoods.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.ProductName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(i => i.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            // TotalPrice é uma propriedade calculada deve ser ignorada pois é para visão
            builder.Ignore(i => i.TotalPrice);
        }
    }
}
