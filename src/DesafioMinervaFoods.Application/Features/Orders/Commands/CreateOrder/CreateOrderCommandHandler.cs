using AutoMapper;
using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentValidation;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderRquestedResponse>>
    {
        private readonly IOrderRepository _repository;
        private readonly IValidator<CreateOrderCommand> _validator;
        private readonly IMapper _mapper;
        private readonly IEventBus _eventBus;

        public CreateOrderCommandHandler(
            IOrderRepository repository,
            IValidator<CreateOrderCommand> validator,
            IMapper mapper,
            IEventBus eventBus)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _eventBus = eventBus;
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

            await _eventBus.PublishAsync(new RegisterOrderCommand(
                request.CustomerId,
                request.PaymentConditionId,
                request.Items
            ), cancellationToken);


            return Result<OrderRquestedResponse>.Success(new OrderRquestedResponse("Pedido solicitado com sucesso!"));
        }
    }
}
