namespace DesafioMinervaFoods.Application.Events
{
    public record OrderCreatedEvent(Guid OrderId, DateTime OrderDate);
}
