using DesafioMinervaFoods.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DesafioMinervaFoods.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Realiza a autenticação do usuário.
        /// </summary>
        /// <param name="request">Credenciais de acesso (usuário e senha).</param>
        /// <response code="200">Autenticação realizada com sucesso, retorna o token JWT.</response>
        /// <response code="401">Credenciais inválidas.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TODO: Implementar lógica de geração de token posteriormente
            if (request.Username == "admin" && request.Password == "123456")
            {
                return Ok(new { token = "TOKEN_PROVISORIO" });
            }

            return Unauthorized(new { mensagem = "Usuário ou senha inválidos." });
        }
    }
}