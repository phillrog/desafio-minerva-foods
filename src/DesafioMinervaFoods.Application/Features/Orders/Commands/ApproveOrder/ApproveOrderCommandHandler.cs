using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Commands.ApproveOrder
{
    public class ApproveOrderCommandHandler : IRequestHandler<ApproveOrderCommand, Result<ProcessOrderApprovalResponse>>
    {
        private readonly IOrderRepository _repository;
        private readonly IEventBus _eventBus;
        private readonly ICurrentUserService _currentUserService;

        public ApproveOrderCommandHandler(IOrderRepository repository, 
            IEventBus eventBus, 
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _eventBus = eventBus;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ProcessOrderApprovalResponse>> Handle(ApproveOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                return Result<ProcessOrderApprovalResponse>.Failure("Pedido não encontrado ou não requer aprovação.");
            }

            // O pedido precisa de aprovação?
            if (!order.RequiresManualApproval)
            {
                return Result<ProcessOrderApprovalResponse>.Failure("Este pedido não requer aprovação manual ou já foi processado.");
            }

            var userId = _currentUserService.UserId ?? Guid.Empty;

            // coloca na fila
            await _eventBus.PublishAsync(new ProcessOrderApprovalCommand(
                request.OrderId,
                userId
            ), cancellationToken);

            // retorna para usuário
            return Result<ProcessOrderApprovalResponse>.Success(
                new ProcessOrderApprovalResponse("Solicitação de aprovação enviada com sucesso!")
            );
        }
    }
}