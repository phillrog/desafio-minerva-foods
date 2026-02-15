namespace DesafioMinervaFoods.Application.DTOs
{
    /// <summary>
    /// Resposta de customer.
    /// </summary>
    public record CustomerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; init; }        
    }
}
