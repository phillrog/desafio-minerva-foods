using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Features.Auth.Queries.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DesafioMinervaFoods.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza a autenticação do usuário e retorna um token JWT.
        /// </summary>
        /// <remarks>
        /// Utilize o e-mail 'avaliador@minervafoods.com.br' e a senha configurada no Seed (ex: '1234') para testar.
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
            var query = new LoginQuery(request.Username, request.Password);
            var result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Errors.FirstOrDefault() });

            return Ok(result.Data);
        }
    }
}