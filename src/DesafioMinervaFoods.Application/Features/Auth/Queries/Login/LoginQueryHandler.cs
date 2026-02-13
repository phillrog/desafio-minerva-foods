using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Interfaces;
using MediatR;

namespace DesafioMinervaFoods.Application.Features.Auth.Queries.Login
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, Result<LoginResponse>>
    {
        private readonly IIdentityService _identityService;

        public LoginQueryHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            return await _identityService.AuthenticateAsync(request.Username, request.Password);
        }
    }
}
