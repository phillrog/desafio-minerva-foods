using DesafioMinervaFoods.Application.DTOs;

namespace DesafioMinervaFoods.Application.Interfaces
{
    public interface ITokenService
    {
        LoginResponse GenerateToken(string email, IList<string> roles);
    }
}
