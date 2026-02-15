using AutoMapper;
using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.PaymentCondition.Queries.GetAllOrders
{
    public class GetAllPaymentConditionQueryHandler : IRequestHandler<GetAllPaymentConditionQuery, Result<IEnumerable<PaymentConditionResponse>>>
    {
        private readonly IPaymentConditionRepository _repository;
        private readonly IMapper _mapper;

        public GetAllPaymentConditionQueryHandler(IPaymentConditionRepository repository, IMapper _mapper)
        {
            _repository = repository;
            this._mapper = _mapper;
        }

        public async Task<Result<IEnumerable<PaymentConditionResponse>>> Handle(GetAllPaymentConditionQuery request, CancellationToken cancellationToken)
        {
            var paymentsConditions = await _repository.GetAllAsync();

            var response = _mapper.Map<IEnumerable<PaymentConditionResponse>>(paymentsConditions);

            return Result<IEnumerable<PaymentConditionResponse>>.Success(response);
        }
    }
}
