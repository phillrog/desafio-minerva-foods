using DesafioMinervaFoods.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DesafioMinervaFoods.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(o => o.OrderId);

            builder.Property(o => o.TotalAmount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(o => o.Status)
                .HasConversion<string>() // Salva o Enum como String no banco
                .HasMaxLength(30);

            // Relacionamento 1:N com itens
            // Se deletar o pedido, deleta os itens (Cascade)
            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.CreatedAt).IsRequired();
        }
    }
}
