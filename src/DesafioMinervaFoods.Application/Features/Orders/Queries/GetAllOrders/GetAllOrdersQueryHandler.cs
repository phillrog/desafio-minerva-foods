using AutoMapper;
using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, Result<IEnumerable<OrderResponse>>>
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public GetAllOrdersQueryHandler(IOrderRepository repository, IMapper _mapper)
        {
            _repository = repository;
            this._mapper = _mapper;
        }

        public async Task<Result<IEnumerable<OrderResponse>>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _repository.GetAllAsync();

            var response = _mapper.Map<IEnumerable<OrderResponse>>(orders);

            return Result<IEnumerable<OrderResponse>>.Success(response);
        }
    }
}
