using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Infrastructure.Persistence;
using MassTransit;

namespace DesafioMinervaFoods.Infrastructure.Consumers
{
    public class ApproveOrderConsumer : IConsumer<ProcessOrderApprovalCommand>
    {
        private readonly AppDbContext _context;
        private readonly IOrderRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;

        public ApproveOrderConsumer(
            AppDbContext context,
            IOrderRepository repository,
            IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<ProcessOrderApprovalCommand> context)
        {
            var msg = context.Message;
            var order = await _repository.GetByIdAsync(msg.OrderId);

            if (order != null && order.RequiresManualApproval)
            {
                order.Aprovar();

                await _repository.UpdateAsync(order);
                await _context.SaveChangesAsync();

                // Notifica o sistema que o status mudou 
                await _publishEndpoint.Publish(new OrderProcessedEvent(
                    order.Id,
                    order.CustomerId,
                    "Sucesso",
                    "Pedido aprovado com sucesso e enviado para processamento!"
                ));
            }
        }
    }
}
