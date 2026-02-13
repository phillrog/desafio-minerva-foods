using MediatR;
using DesafioMinervaFoods.Application.Common.Interfaces;

namespace DesafioMinervaFoods.Application.Common.Behaviors
{
    /// <summary>
    /// Pipeline Behavior responsável por garantir a atomicidade das operações de escrita (Commands).
    /// Implementa o padrão Unit of Work de forma centralizada no MediatR.
    /// </summary>
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Estratégia de Identificação: Apenas classes terminadas em 'Command' abrem transação.
            // Isso evita overhead desnecessário em Queries (leitura), otimizando a performance do banco.
            bool isCommand = request.GetType().Name.EndsWith("Command");

            if (!isCommand)
            {
                return await next();
            }

            try
            {
                // Inicia o escopo transacional antes de entrar no Handler
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                // Executa a lógica de negócio dentro do Handler correspondente
                var response = await next();

                // Persiste todas as alterações rastreadas pelo EF Core no banco de dados.
                // Se o SaveChanges falhar, o fluxo cai no catch e nada é comitado.
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Finaliza a transação atômica no SQL Server
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return response;
            }
            catch (Exception)
            {
                // Em caso de falha em qualquer parte do processo, reverte todas as alterações.
                // Garante que o banco nunca fique em um estado inconsistente (ex: pedido sem itens).
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                // Relança a exceção para ser tratada pelo middleware global de exceções
                throw;
            }
        }
    }
}