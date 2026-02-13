using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DesafioMinervaFoods.Tests.Infrastructure.Repositories
{
    public class CustomerRepositoryTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;

        public CustomerRepositoryTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
        }

        private AppDbContext CriarContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Banco novo para cada teste
                .Options;

            return new AppDbContext(options, _currentUserServiceMock.Object);
        }

        [Fact]
        public async Task Deve_RetornarTrue_Quando_ClienteExistirNoBanco()
        {
            // Arrange
            var context = CriarContextoInMemory();
            var cliente = new Customer("Minerva Foods", "contato@minervafoods.com");
            var customerIdGerado = cliente.Id; 

            context.Set<Customer>().Add(cliente);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            // Act
            var existe = await repository.ExistsAsync(customerIdGerado);

            // Assert
            existe.Should().BeTrue();
        }

        [Fact]
        public async Task Deve_RetornarFalse_Quando_ClienteNaoExistirNoBanco()
        {
            // Arrange
            var context = CriarContextoInMemory();
            var repository = new CustomerRepository(context);
            var idAleatorio = Guid.NewGuid();

            // Act
            var existe = await repository.ExistsAsync(idAleatorio);

            // Assert
            existe.Should().BeFalse();
        }
    }
}