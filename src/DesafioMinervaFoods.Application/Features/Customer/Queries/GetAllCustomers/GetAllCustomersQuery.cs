using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Customer.Queries.GetAllOrders
{
    public record GetAllCustomersQuery() : IRequest<Result<IEnumerable<CustomerResponse>>>;
}
