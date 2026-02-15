using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.PaymentCondition.Queries.GetAllOrders
{
    public record GetAllPaymentConditionQuery() : IRequest<Result<IEnumerable<PaymentConditionResponse>>>;
}
