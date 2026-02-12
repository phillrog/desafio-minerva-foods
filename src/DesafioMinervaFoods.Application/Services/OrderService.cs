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

        public async Task<OrderResponse> CreateOrderAsync(OrderCreateRequest request)
        {
            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {            
                throw new ValidationException(validationResult.Errors);
            }

            var items = request.Items.Select(i => new OrderItem(i.ProductName, i.Quantity, i.UnitPrice)).ToList();
            var order = new Order(request.CustomerId, request.PaymentConditionId, items);

            await _repository.AddAsync(order);

            return new OrderResponse(order.OrderId, order.TotalAmount, order.Status.ToString(), order.RequiresManualApproval);
        }

        public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync()
        {
            var orders = await _repository.GetAllAsync();
            return orders.Select(o => new OrderResponse(o.OrderId, o.TotalAmount, o.Status.ToString(), o.RequiresManualApproval));
        }

        public async Task<bool> ApproveOrderAsync(Guid orderId)
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null) return false;

            order.Aprovar();
            await _repository.UpdateAsync(order);
            return true;
        }
    }
}
