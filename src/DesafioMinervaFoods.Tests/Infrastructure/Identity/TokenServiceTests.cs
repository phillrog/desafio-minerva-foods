using DesafioMinervaFoods.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DesafioMinervaFoods.Tests.Infrastructure
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly TokenService _tokenService;
        private const string SecretKey = "chave_secreta_super_longa_para_o_desafio_minerva_foods";

        public TokenServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();

            // Simula a leitura da chave do appsettings.json
            _configurationMock.Setup(c => c["Jwt:Secret"]).Returns(SecretKey);

            _tokenService = new TokenService(_configurationMock.Object);
        }

        [Fact]
        public void Deve_Gerar_Token_Valido_Com_As_Claims_Corretas()
        {
            // Arrange
            var email = "avaliador@minervafoods.com.br";
            var roles = new List<string> { "Admin", "User" };

            // Act
            var response = _tokenService.GenerateToken(It.IsAny<Guid>(), email, roles);

            // Assert
            response.Should().NotBeNull();
            response.Token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(response.Token);

            jwtToken.Claims.Should().Contain(c =>
                (c.Type == ClaimTypes.Email || c.Type == "email") && c.Value == email);

            var claimsRoles = jwtToken.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value);

            claimsRoles.Should().Contain(roles);
        }

        [Fact]
        public void Deve_Definir_Data_De_Expiracao_Corretamente()
        {
            // Arrange
            var email = "test@minerva.com.br";
            var roles = new List<string>();

            // Act
            var response = _tokenService.GenerateToken(It.IsAny<Guid>(), email, roles);

            // Assert
            // Como é AddHours(2), a expiração deve ser maior que agora
            response.ExpiresIn.Should().BeAfter(DateTime.UtcNow);
            response.ExpiresIn.Should().BeBefore(DateTime.UtcNow.AddHours(2).AddMinutes(1));
        }
    }
}
