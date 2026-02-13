namespace DesafioMinervaFoods.Application.Events
{
    public record ProcessOrderApprovalCommand(Guid OrderId, Guid UserId);
}
