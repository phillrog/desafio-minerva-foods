using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Infrastructure.Persistence;
using MassTransit;

namespace DesafioMinervaFoods.Infrastructure.Consumers
{
    public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly AppDbContext _context;

        public OrderCreatedConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;
            const int DEFAULT_DELIVERY_DAYS = 10;

            // Simulação da regra de negócio: Prazo de 10 dias
            var deliveryTerm = new DeliveryTerm(message.OrderId, DEFAULT_DELIVERY_DAYS, message.OrderDate);

            _context.DeliveryTerms.Add(deliveryTerm);
            await _context.SaveChangesAsync();
        }
    }
}
