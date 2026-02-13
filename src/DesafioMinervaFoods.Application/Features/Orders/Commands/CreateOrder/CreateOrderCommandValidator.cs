using DesafioMinervaFoods.Domain.Interfaces.Repositories;
using FluentValidation;

namespace DesafioMinervaFoods.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator(
            ICustomerRepository customerRepository,
            IPaymentConditionRepository paymentConditionRepository)
        {
            // Validação de Existência do Cliente
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("O cliente é obrigatório.")
                .MustAsync(async (id, cancellation) => await customerRepository.ExistsAsync(id))
                .WithMessage("O cliente informado não existe na base de dados.");

            // Validação de Existência da Condição de Pagamento
            RuleFor(x => x.PaymentConditionId)
                .NotEmpty().WithMessage("A condição de pagamento é obrigatória.")
                .MustAsync(async (id, cancellation) => await paymentConditionRepository.ExistsAsync(id))
                .WithMessage("A condição de pagamento informada não existe na base de dados.");

            // Validação dos Itens
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("O pedido deve ter pelo menos um item.");

            RuleForEach(x => x.Items).ChildRules(item => {
                item.RuleFor(i => i.ProductName)
                    .NotEmpty().WithMessage("O nome do produto é obrigatório.");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");

                item.RuleFor(i => i.UnitPrice)
                    .GreaterThan(0).WithMessage("O preço unitário deve ser maior que zero.");
            });
        }
    }
}