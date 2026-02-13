using DesafioMinervaFoods.Application.Common.Interfaces;
using MassTransit;

namespace DesafioMinervaFoods.Infrastructure.Messaging
{
    public class MassTransitEventBus : IEventBus
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventBus(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
            where T : class
        {
            await _publishEndpoint.Publish(@event, cancellationToken);
        }
    }
}
