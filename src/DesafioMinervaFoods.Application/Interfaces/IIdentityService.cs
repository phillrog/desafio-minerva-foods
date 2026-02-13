using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;

namespace DesafioMinervaFoods.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<LoginResponse>> AuthenticateAsync(string email, string password);
    }
}
