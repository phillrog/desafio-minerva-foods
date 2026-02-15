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

        [Fact]
        public async Task Deve_RetornarTodosOsClientes_Quando_ExistiremNoBanco()
        {
            // Arrange
            var context = CriarContextoInMemory();

            // Criando alguns clientes para o teste
            var cliente1 = new Customer("Cliente A", "a@teste.com");
            var cliente2 = new Customer("Cliente B", "b@teste.com");

            await context.Customers.AddRangeAsync(cliente1, cliente2);
            await context.SaveChangesAsync();

            var repository = new CustomerRepository(context);

            // Act
            var clientes = await repository.GetAllAsync();

            // Assert
            clientes.Should().NotBeNull();
            clientes.Should().HaveCount(2);
            clientes.Should().Contain(c => c.Name == "Cliente A");
            clientes.Should().Contain(c => c.Name == "Cliente B");
        }

        [Fact]
        public async Task Deve_RetornarListaVazia_Quando_NaoHouverClientes()
        {
            // Arrange
            var context = CriarContextoInMemory(); // Banco novo/vazio
            var repository = new CustomerRepository(context);

            // Act
            var clientes = await repository.GetAllAsync();

            // Assert
            clientes.Should().NotBeNull();
            clientes.Should().BeEmpty();
        }
    }
}