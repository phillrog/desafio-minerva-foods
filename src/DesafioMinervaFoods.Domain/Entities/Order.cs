using DesafioMinervaFoods.Domain.Enums;

namespace DesafioMinervaFoods.Domain.Entities
{
    public class Order
    {
        public Guid OrderId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Guid PaymentConditionId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public StatusEnum Status { get; private set; }
        public bool RequiresManualApproval { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public List<OrderItem> Items { get; private set; } = new();
        public virtual DeliveryTerm? DeliveryTerm { get; private set; }

        public Order() {}
        public Order(Guid customerId, Guid paymentConditionId, List<OrderItem> items)
        {
            OrderId = Guid.NewGuid();
            CustomerId = customerId;
            PaymentConditionId = paymentConditionId;
            OrderDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            Items = items;

            TotalAmount = items.Sum(x => x.TotalPrice);
            AtualizaStatusPedido();
        }

        private void AtualizaStatusPedido()
        {
            if (TotalAmount > 5000)
            {
                Status = StatusEnum.Criado;
                RequiresManualApproval = true;
            }
            else
            {
                Status = StatusEnum.Pago;
                RequiresManualApproval = false;
            }
        }

        public void Aprovar()
        {
            if (RequiresManualApproval)
            {
                Status = StatusEnum.Pago;
                RequiresManualApproval = false;
            }
        }

        public void Cancelar()
        {
            Status = StatusEnum.Cancelado;
            RequiresManualApproval = false;            
        }

        public void DefinirPrazoEntrega(DeliveryTerm deliveryTerm)
        {
            if (deliveryTerm.OrderId != this.OrderId)
                throw new InvalidOperationException("O prazo de entrega não pertence a este pedido.");

            DeliveryTerm = deliveryTerm;
        }
    }
}
