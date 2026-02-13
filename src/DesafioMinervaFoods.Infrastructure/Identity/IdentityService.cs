using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DesafioMinervaFoods.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;

        public IdentityService(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<LoginResponse>> AuthenticateAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var response = _tokenService.GenerateToken(user.Email!, roles);
                return Result<LoginResponse>.Success(response);
            }

            return Result<LoginResponse>.Failure("Usuário ou senha inválidos.");
        }
    }
}
