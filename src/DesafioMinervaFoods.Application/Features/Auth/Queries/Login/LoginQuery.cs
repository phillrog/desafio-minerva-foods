using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Auth.Queries.Login
{
    public record LoginQuery(string Username, string Password) : IRequest<Result<LoginResponse>>;
}
