using DesafioMinervaFoods.Application.DTOs;

namespace DesafioMinervaFoods.Application.Interfaces
{
    public interface ITokenService
    {
        LoginResponse GenerateToken(Guid usuarioId, string email, IList<string> roles);
    }
}
