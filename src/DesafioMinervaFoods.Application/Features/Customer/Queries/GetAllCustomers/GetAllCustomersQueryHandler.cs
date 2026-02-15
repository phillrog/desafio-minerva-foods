using AutoMapper;
using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Customer.Queries.GetAllOrders
{
    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, Result<IEnumerable<CustomerResponse>>>
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public GetAllCustomersQueryHandler(ICustomerRepository repository, IMapper _mapper)
        {
            _repository = repository;
            this._mapper = _mapper;
        }

        public async Task<Result<IEnumerable<CustomerResponse>>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _repository.GetAllAsync();

            var response = _mapper.Map<IEnumerable<CustomerResponse>>(customers);

            return Result<IEnumerable<CustomerResponse>>.Success(response);
        }
    }
}
