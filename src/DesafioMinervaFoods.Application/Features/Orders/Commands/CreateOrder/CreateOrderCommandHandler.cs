using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Events;
using FluentValidation;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderRquestedResponse>>
    {
        private readonly IValidator<CreateOrderCommand> _validator;
        private readonly IEventBus _eventBus;
        private readonly ICurrentUserService _currentUserService;

        public CreateOrderCommandHandler(
            IValidator<CreateOrderCommand> validator,
            IEventBus eventBus,
            ICurrentUserService currentUserService)
        {
            _validator = validator;
            _eventBus = eventBus;
            _currentUserService = currentUserService;
        }

        public async Task<Result<OrderRquestedResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Validação
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<OrderRquestedResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var orderId = Guid.NewGuid(); // Você gera o ID aqui para devolver ao usuário
            var userId = _currentUserService.UserId ?? Guid.Empty;

            await _eventBus.PublishAsync(new RegisterOrderCommand(
                request.CustomerId,
                request.PaymentConditionId,
                request.Items,
                userId
            ), cancellationToken);


            return Result<OrderRquestedResponse>.Success(new OrderRquestedResponse("Pedido solicitado com sucesso!"));
        }
    }
}
