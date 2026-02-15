using DesafioMinervaFoods.Application.Common.Interfaces;
using DesafioMinervaFoods.Domain.Entities;
using DesafioMinervaFoods.Infrastructure.Persistence;
using DesafioMinervaFoods.Infrastructure.Persistence.Repositories;
using FluentAssertions;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Reflection;

namespace DesafioMinervaFoods.Tests.Infrastructure
{
    public class OrderRepositoryTests
    {
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        public OrderRepositoryTests()
        {
            _currentUserServiceMock = new Mock<ICurrentUserService>();
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task AddAsync_Deve_PersistirPedidoNoBancoInMemory()
        {
            // Arrange
            using var context = new AppDbContext(_dbContextOptions, _currentUserServiceMock.Object);
            var repository = new OrderRepository(context);
            var order = new Order(Guid.NewGuid(), Guid.NewGuid(),
                new List<OrderItem> { new("Teste", 1, 100) });

            // Act
            await repository.AddAsync(order);

            // Assert
            var result = await context.Orders.FirstOrDefaultAsync(o => o.Id == order.Id);
            result.Should().NotBeNull();
            result!.TotalAmount.Should().Be(100);
        }

        [Fact]
        public async Task GetAllAsync_QuandoUserIdForInformado_DeveRetornarApenasPedidosDoUsuario()
        {
            // 1. Arrange - Configuração estável do banco
            var userId = Guid.NewGuid();
            var dbName = Guid.NewGuid().ToString(); // Nome único para este teste
            
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            _currentUserServiceMock.Setup(s => s.UserId).Returns(userId);

            // 2. Gravação (Escopo isolado)
            using (var contextGrava = new AppDbContext(options, _currentUserServiceMock.Object))
            {
                var customer = new Customer();
                var payment = new PaymentCondition();

                SetPrivateProperty(customer, nameof(Customer.Id), Guid.NewGuid());
                SetPrivateProperty(customer, nameof(Customer.Name), "Teste");
                SetPrivateProperty(customer, nameof(Customer.Email), "t@t.com");
                SetPrivateProperty(payment, nameof(PaymentCondition.Id), Guid.NewGuid());
                SetPrivateProperty(payment, nameof(PaymentCondition.Description), "À Vista");
                SetPrivateProperty(payment, nameof(PaymentCondition.NumberOfInstallments), 1);

                await contextGrava.Customers.AddAsync(customer);
                await contextGrava.PaymentConditions.AddAsync(payment);

                var order = new Order(userId, Guid.NewGuid(), new List<OrderItem> { new("Teste", 1, 100) });
                
                SetPrivateProperty(order, nameof(Order.Customer), customer);
                SetPrivateProperty(order, nameof(Order.PaymentCondition), payment);

                // Vamos ignorar o OnBeforeSaving por um segundo e setar na mão via Entidade
                // Se sua entidade tiver a propriedade física, use ela. Se não, use o Property do EF:
                contextGrava.Orders.Add(order);
                contextGrava.Entry(order).Property("CreatedBy").CurrentValue = userId;

                await contextGrava.SaveChangesAsync();
            }

            // 3. Act & Assert (Novo escopo, mesma 'options' e 'dbName')
            using (var contextLeitura = new AppDbContext(options, _currentUserServiceMock.Object))
            {
                // PASSO DE DIAGNÓSTICO REAL:
                var contagemBruta = await contextLeitura.Orders.CountAsync();
                if (contagemBruta == 0)
                    throw new Exception("O banco em memória ESTÁ VAZIO.");

                var repository = new OrderRepository(contextLeitura);
                var result = await repository.GetAllAsync(userId);

                // Assert
                result.Should().NotBeEmpty();
                result.First().CreatedBy.Should().Be(userId);
            }
        }

        [Fact]
        public async Task GetAllAsync_QuandoUserIdForNulo_DeveRetornarTodosOsPedidos()
        {
            // 1. Arrange - Configuração de isolamento
            var dbName = Guid.NewGuid().ToString();
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            // 2. Gravação (Escopo isolado)
            using (var contextGrava = new AppDbContext(options, _currentUserServiceMock.Object))
            {
                // Customer e Payment para os pedidos não quebrarem nos Includes
                var customer = new Customer();
                var payment = new PaymentCondition();
                SetPrivateProperty(customer, nameof(Customer.Id), Guid.NewGuid());
                SetPrivateProperty(customer, nameof(Customer.Name), "Teste");
                SetPrivateProperty(customer, nameof(Customer.Email), "t@t.com");
                SetPrivateProperty(payment, nameof(PaymentCondition.Id), Guid.NewGuid());
                SetPrivateProperty(payment, nameof(PaymentCondition.Description), "À Vista");
                SetPrivateProperty(payment, nameof(PaymentCondition.NumberOfInstallments), 1);

                await contextGrava.Customers.AddAsync(customer);
                await contextGrava.PaymentConditions.AddAsync(payment);

                // Criando dois pedidos de usuários diferentes
                var order1 = new Order(Guid.NewGuid(), customer.Id, new List<OrderItem> { new("I1", 1, 10) });
                var order2 = new Order(Guid.NewGuid(), customer.Id, new List<OrderItem> { new("I2", 1, 20) });

                // Associando as propriedades de navegação para satisfazer os Includes
                SetPrivateProperty(order1, nameof(Order.Customer), customer);
                SetPrivateProperty(order1, nameof(Order.PaymentCondition), payment);
                SetPrivateProperty(order2, nameof(Order.Customer), customer);
                SetPrivateProperty(order2, nameof(Order.PaymentCondition), payment);

                await contextGrava.Orders.AddRangeAsync(order1, order2);
                await contextGrava.SaveChangesAsync();
            }

            // 3. Act & Assert
            using (var contextLeitura = new AppDbContext(options, _currentUserServiceMock.Object))
            {
                var repository = new OrderRepository(contextLeitura);

                // Act - Passando null para trazer tudo de todos
                var result = await repository.GetAllAsync(null);

                // Assert
                result.Should().NotBeNull();
                result.Should().HaveCount(2, "O repositório deveria retornar todos os pedidos quando o UserId é nulo");
            }
        }

        [Fact]
        public async Task GetByIdAsync_QuandoUsuarioDiferente_DeveRetornarNulo()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var ownerId = Guid.NewGuid();
            var strangerId = Guid.NewGuid();

            using (var context = new AppDbContext(_dbContextOptions, _currentUserServiceMock.Object))
            {
                var order = new Order(ownerId, Guid.NewGuid(), new List<OrderItem> { new("Item", 1, 10) });
                // Forçando o ID para o teste
                typeof(Order).GetProperty("Id")?.SetValue(order, orderId); 
                
                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }

            using (var contextLeitura = new AppDbContext(_dbContextOptions, _currentUserServiceMock.Object))
            {
                var repository = new OrderRepository(contextLeitura);

                // Act
                var result = await repository.GetByIdAsync(orderId, strangerId);

                // Assert
                result.Should().BeNull();
            }
        }

        /// <summary>
        /// Helper para setar propriedades privadas/somente leitura durante os testes
        /// </summary>
        private static void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            prop?.SetValue(obj, value);
        }
    }
}