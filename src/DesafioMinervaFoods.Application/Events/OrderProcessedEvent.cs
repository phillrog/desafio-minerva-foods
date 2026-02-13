using DesafioMinervaFoods.Domain.Entities;

namespace DesafioMinervaFoods.Application.Events
{
    public record OrderProcessedEvent
    {
        public OrderProcessedEvent() { }

        public OrderProcessedEvent(Guid id, Guid customerId, string title, string message)
        {
            Id = id;
            CustomerId = customerId;
            Title = title;
            Message = message;
        }

        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public string Title { get; init; }
        public string Message { get; init; }
    }
}
