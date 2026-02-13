using DesafioMinervaFoods.Domain.Enums;

namespace DesafioMinervaFoods.Domain.Entities
{
    public class Order : Entity<Guid>
    {
        public Guid CustomerId { get; private set; }
        public Guid PaymentConditionId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public StatusEnum Status { get; private set; }
        public bool RequiresManualApproval { get; private set; }

        // Relacionamentos
        public List<OrderItem> Items { get; private set; } = new();
        public virtual DeliveryTerm? DeliveryTerm { get; private set; }
        public virtual Customer Customer { get; private set; }
        public virtual PaymentCondition PaymentCondition { get; private set; }

        // Construtor protegido para o EF
        protected Order() { }

        public Order(Guid customerId, Guid paymentConditionId, List<OrderItem> items)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            PaymentConditionId = paymentConditionId;
            OrderDate = DateTime.UtcNow;
            Items = items;
            TotalAmount = items.Sum(x => x.TotalPrice);
            AtualizaStatusPedido();
        }

        private void AtualizaStatusPedido()
        {
            // Regra da Minerva Foods: Pedidos acima de 5000 precisam de aprovação
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
            if (deliveryTerm.OrderId != this.Id)
                throw new InvalidOperationException("O prazo de entrega não pertence a este pedido.");

            DeliveryTerm = deliveryTerm;
        }

        public void DefinirStatusProcessando()
        {
            Status = StatusEnum.Processando;
        }        
    }
}