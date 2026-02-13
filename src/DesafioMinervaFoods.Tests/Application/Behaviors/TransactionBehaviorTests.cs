using DesafioMinervaFoods.Application.Common.Behaviors;
using DesafioMinervaFoods.Application.Common.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;

namespace DesafioMinervaFoods.Tests.Application.Common.Behaviors
{
    public class TransactionBehaviorTests
    {
        private readonly Mock<IUnitOfWork> _uowMock;
        private readonly TransactionBehavior<TestCommand, string> _behavior;

        public TransactionBehaviorTests()
        {
            _uowMock = new Mock<IUnitOfWork>();
            _behavior = new TransactionBehavior<TestCommand, string>(_uowMock.Object);
        }

        [Fact]
        public async Task Deve_Executar_Commit_Quando_Handler_Tiver_Sucesso()
        {
            // Arrange
            var command = new TestCommand();
            RequestHandlerDelegate<string> next = async (_) =>
            {
                await Task.CompletedTask; // Simula trabalho assíncrono
                return "Sucesso";
            };

            // Act
            var result = await _behavior.Handle(command, next, CancellationToken.None);

            // Assert
            result.Should().Be("Sucesso");
            _uowMock.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Executar_Rollback_Quando_Handler_Lancar_Excecao()
        {
            // Arrange
            var command = new TestCommand();
            RequestHandlerDelegate<string> next = (_) => throw new Exception("Erro no Handler");

            // Act
            Func<Task> act = async () => await _behavior.Handle(command, next, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Erro no Handler");
            _uowMock.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Deve_Ignorar_Transacao_Quando_Nao_For_Um_Command()
        {
            // Arrange
            var query = new TestQuery(); // Não termina com "Command"
            var queryBehavior = new TransactionBehavior<TestQuery, string>(_uowMock.Object);
            RequestHandlerDelegate<string> next = (_) => Task.FromResult("Dados");

            // Act
            await queryBehavior.Handle(query, next, CancellationToken.None);

            // Assert
            _uowMock.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }

    // Classes auxiliares para o teste
    public class TestCommand : IRequest<string> { }
    public class TestQuery : IRequest<string> { }
}