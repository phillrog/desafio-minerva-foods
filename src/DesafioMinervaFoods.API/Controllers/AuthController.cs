using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DesafioMinervaFoods.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Realiza a autenticação do usuário e retorna um token JWT.
        /// </summary>
        /// <remarks>
        /// Utilize o e-mail 'avaliador@minerva.com.br' e a senha configurada no Seed (ex: '1234') para testar.
        /// O token retornado deve ser utilizado no cabeçalho 'Authorization' como 'Bearer {token}'.
        /// </remarks>
        /// <param name="request">Credenciais de acesso (Username deve ser o e-mail).</param>
        /// <response code="200">Autenticação realizada com sucesso. Retorna o token e informações do usuário.</response>
        /// <response code="401">Credenciais inválidas ou usuário não encontrado.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Valida usuário
            var user = await _userManager.FindByEmailAsync(request.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var roles = await _userManager.GetRolesAsync(user);
                var response = _tokenService.GenerateToken(user.Email!, roles);

                return Ok(response);
            }

            return Unauthorized(new { message = "Usuário ou senha inválidos." });
        }
    }
}