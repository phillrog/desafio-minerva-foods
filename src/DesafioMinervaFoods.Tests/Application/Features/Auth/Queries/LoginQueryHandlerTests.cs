using DesafioMinervaFoods.Application.Common;
using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Features.Auth.Queries.Login;
using DesafioMinervaFoods.Application.Interfaces;
using FluentAssertions;
using Moq;

namespace DesafioMinervaFoods.Tests.Application.Features.Auth.Queries
{
    public class LoginQueryHandlerTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly LoginQueryHandler _handler;

        public LoginQueryHandlerTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _handler = new LoginQueryHandler(_identityServiceMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarSucesso_Quando_CredenciaisForemValidas()
        {
            // Arrange
            var loginResponse = new LoginResponse
            {
                Token = "jwt_token_fake",
                Username = "avaliador@minervafoods.com.br"
            };

            _identityServiceMock
                .Setup(s => s.AuthenticateAsync("avaliador@minervafoods.com.br", "1234"))
                .ReturnsAsync(Result<LoginResponse>.Success(loginResponse));

            var query = new LoginQuery("avaliador@minervafoods.com.br", "1234");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Token.Should().Be("jwt_token_fake");
            _identityServiceMock.Verify(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_CredenciaisForemIncorretas()
        {
            // Arrange
            _identityServiceMock
                .Setup(s => s.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(Result<LoginResponse>.Failure("Usuário ou senha inválidos."));

            var query = new LoginQuery("errado@minervafoods.com.br", "0000");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Usuário ou senha inválidos.");
        }
    }
}
