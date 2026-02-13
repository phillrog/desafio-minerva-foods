using AutoMapper;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using DesafioMinervaFoods.Infrastructure.Persistence;
using MassTransit;

namespace DesafioMinervaFoods.Infrastructure.Consumers
{

    public class RegisterOrderConsumer : IConsumer<RegisterOrderCommand>
    {
        private readonly AppDbContext _context;
        private readonly IOrderRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;

        public RegisterOrderConsumer(
            AppDbContext context,
            IOrderRepository repository,
            IPublishEndpoint publishEndpoint,
            IMapper mapper)
        {
            _context = context;
            _repository = repository;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<RegisterOrderCommand> context)
        {
            var msg = context.Message;
            var order = _mapper.Map<Order>(msg);

            order.DefinirStatusProcessando();

            await _repository.AddAsync(order);
            await _context.SaveChangesAsync();

            await _publishEndpoint.Publish(new OrderCreatedEvent(
                order.Id,
                order.OrderDate));

            await _publishEndpoint.Publish(new OrderProcessedEvent(
                order.Id,
                order.CustomerId, 
                "Sucesso",
                "Pedido gerado com sucesso!"
            ));
        }
    }
}
