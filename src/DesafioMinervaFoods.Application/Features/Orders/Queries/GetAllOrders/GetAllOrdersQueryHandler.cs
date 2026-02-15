using AutoMapper;
using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<IEnumerable<OrderResponse>>>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public GetAllOrdersQueryHandler(IOrderRepository repository
            , IMapper _mapper
            , ICurrentUserService currentUserService)
        {
            _repository = repository;
            this._mapper = _mapper;
            _currentUserService = currentUserService;
        }

        public async Task<Result<IEnumerable<OrderResponse>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            var orders = await _repository.GetAllAsync(userId);
            var response = _mapper.Map<IEnumerable<OrderResponse>>(orders);

            return Result<IEnumerable<OrderResponse>>.Success(response);
        }
    }
}
