using DesafioMinervaFoods.Application.DTOs;
using FluentValidation;

namespace DesafioMinervaFoods.Application.Validations
{
    public class OrderCreateValidator : AbstractValidator<OrderCreateRequest>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(x => x.PaymentConditionId).NotEmpty();
            RuleFor(x => x.Items).NotEmpty().WithMessage("O pedido deve ter itens.");
            RuleForEach(x => x.Items).ChildRules(item => {
                item.RuleFor(i => i.Quantity).GreaterThan(0);
                item.RuleFor(i => i.UnitPrice).GreaterThan(0);
            });
        }
    }
}
