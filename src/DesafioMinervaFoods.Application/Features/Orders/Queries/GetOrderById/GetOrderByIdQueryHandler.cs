using AutoMapper;
using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetOrderByIdQueryHandler(IOrderRepository repository
            , IMapper mapper
            , ICurrentUserService currentUserService)
        {
            _repository = repository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var order = await _repository.GetByIdAsync(request.OrderId, userId);

            if (order is null)
            {
                return Result<OrderResponse>.Failure("Pedido não encontrado.");
            }

            var response = _mapper.Map<OrderResponse>(order);

            return Result<OrderResponse>.Success(response);
        }
    }
}
