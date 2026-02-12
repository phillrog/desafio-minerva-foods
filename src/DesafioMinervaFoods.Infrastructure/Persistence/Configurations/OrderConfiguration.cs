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

            builder.HasOne<Customer>()
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .IsRequired() 
                .OnDelete(DeleteBehavior.Restrict); // Evita deletar cliente se houver pedidos

            // Relacionamento com PaymentCondition (Obrigatório)
            builder.HasOne<PaymentCondition>()
                .WithMany()
                .HasForeignKey(o => o.PaymentConditionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(o => o.CreatedAt).IsRequired();
        }
    }
}
