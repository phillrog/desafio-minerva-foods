using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Commands.ApproveOrder
{
    public record ApproveOrderCommand(Guid OrderId) : IRequest<Result<ProcessOrderApprovalResponse>>;
}
