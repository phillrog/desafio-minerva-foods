namespace DesafioMinervaFoods.Application.DTOs
{
    /// <summary>
    /// Resposta de autenticação contendo o token de acesso.
    /// </summary>
    public record LoginResponse{
        public string Token { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public DateTime ExpiresIn { get; init; }
        public IList<string> Roles { get; init; } = new List<string>();
    }
}
