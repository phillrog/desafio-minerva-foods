using DesafioMinervaFoods.Application.DTOs;
using DesafioMinervaFoods.Application.Interfaces;
using DesafioMinervaFoods.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace DesafioMinervaFoods.Tests.Infrastructure
{
    public class IdentityServiceTests
    {
        private readonly Mock<UserManager<IdentityUser>> _userManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly IdentityService _service;

        public IdentityServiceTests()
        {
            // O UserManager precisa de um UserStore mockado para ser instanciado
            var userStoreMock = new Mock<IUserStore<IdentityUser>>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(
                userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _tokenServiceMock = new Mock<ITokenService>();
            _service = new IdentityService(_userManagerMock.Object, _tokenServiceMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarSucesso_Quando_Usuario_E_Senha_Estiverem_Corretos()
        {
            // Arrange
            var email = "avaliador@minervafoods.com.br";
            var password = "SenhaValida123";
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                Email = email,
                UserName = email
            };

            var roles = new List<string> { "Admin" };
            var loginResponse = new LoginResponse { Token = "token_gerado", Username = email };

            _userManagerMock.Setup(u => u.FindByEmailAsync(email))
                .ReturnsAsync(user);

            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, password))
                .ReturnsAsync(true);

            _userManagerMock.Setup(u => u.GetRolesAsync(user))
                .ReturnsAsync(roles);

            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<Guid>(), email, roles))
                .Returns(loginResponse);

            // Act
            var result = await _service.AuthenticateAsync(email, password);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Token.Should().Be("token_gerado");
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_Usuario_Nao_For_Encontrado()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((IdentityUser)null!);

            // Act
            var result = await _service.AuthenticateAsync("invalido@teste.com", "123");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Usuário ou senha inválidos.");
        }

        [Fact]
        public async Task Deve_RetornarFalha_Quando_Senha_For_Incorreta()
        {
            // Arrange
            var user = new IdentityUser { Email = "teste@teste.com" };
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>()))
                .ReturnsAsync(false); // Senha errada

            // Act
            var result = await _service.AuthenticateAsync("teste@teste.com", "senha_errada");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain("Usuário ou senha inválidos.");
        }
    }
}
