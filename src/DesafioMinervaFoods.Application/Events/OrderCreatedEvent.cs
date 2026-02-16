namespace DesafioMinervaFoods.Application.Events
{
    public record OrderCreatedEvent(Guid OrderId, DateTime OrderDate)
    {        
        protected OrderCreatedEvent() : this(Guid.Empty, DateTime.MinValue) { }
    }
}
