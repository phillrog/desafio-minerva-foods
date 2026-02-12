using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Interfaces;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DesafioMinervaFoods.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IValidator<OrderCreateRequest> _validator;
        public OrderService(IOrderRepository repository,
            IValidator<OrderCreateRequest> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<Result<OrderResponse>> CreateOrderAsync(OrderCreateRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Result<OrderResponse>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var items = request.Items.Select(i => new OrderItem(i.ProductName, i.Quantity, i.UnitPrice)).ToList();
            var order = new Order(request.CustomerId, request.PaymentConditionId, items);

            await _repository.AddAsync(order);

            return Result<OrderResponse>.Success(new OrderResponse(
                order.OrderId,
                order.TotalAmount,
                order.Status,
                order.RequiresManualApproval));
        }

        public async Task<Result<IEnumerable<OrderResponse>>> GetAllOrdersAsync()
        {
            var orders = await _repository.GetAllAsync();
            return Result<IEnumerable<OrderResponse>>.Success(orders.Select(o => 
                        new OrderResponse(o.OrderId, o.TotalAmount, o.Status, o.RequiresManualApproval)));
        }

        public async Task<Result> ApproveOrderAsync(Guid orderId)
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                return Result.Failure("Pedido não encontrado ou não requer aprovação.");
            }

            order.Aprovar();
                
            await _repository.UpdateAsync(order);

            return Result.Success();
        }
    }
}
