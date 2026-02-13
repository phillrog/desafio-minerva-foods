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
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderResponse>>
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

        public async Task<Result<OrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Validação
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<OrderResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            // itens
            var items = request.Items.Select(i =>
                new OrderItem(i.ProductName, i.Quantity, i.UnitPrice)).ToList();

            var order = new Order(request.CustomerId, request.PaymentConditionId, items);

            // grava
            await _repository.AddAsync(order);

            // Joga o evento para a fila.             
            await _eventBus.PublishAsync(new OrderCreatedEvent(order.Id, order.OrderDate), cancellationToken);

            var response = _mapper.Map<OrderResponse>(order);

            return Result<OrderResponse>.Success(response);
        }
    }
}
