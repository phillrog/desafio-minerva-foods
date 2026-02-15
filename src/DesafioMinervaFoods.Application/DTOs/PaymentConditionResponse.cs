namespace DesafioMinervaFoods.Application.DTOs
{
    /// <summary>
    /// Resposta de forma de pagamento.
    /// </summary>
    public record PaymentConditionResponse
    {
        public Guid Id { get; set; }
        public string Description { get; init; }
        public int NumberOfInstallments { get; set; }
    }
}
