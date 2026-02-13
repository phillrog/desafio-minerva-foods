using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Commands.ApproveOrder
{
    public class ApproveOrderCommandHandler : IRequestHandler<ApproveOrderCommand, Result>
    {
        private readonly IOrderRepository _repository;

        public ApproveOrderCommandHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result> Handle(ApproveOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.OrderId);

            if (order == null)
            {
                return Result.Failure("Pedido não encontrado ou não requer aprovação.");
            }

            // Regra de Negócio: O pedido precisa de aprovação?
            if (!order.RequiresManualApproval)
                return Result<bool>.Failure("Este pedido não requer aprovação manual.");

            // Aprovar o pedido
            order.Aprovar();

            await _repository.UpdateAsync(order);

            return Result.Success();
        }
    }
}
