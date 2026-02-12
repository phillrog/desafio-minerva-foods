using DesafioMinervaFoods.Application.DTOs;

namespace DesafioMinervaFoods.Application.Interfaces
{
    public interface IOrderService
    {
        // Responsável por criar o pedido, aplicar a regra de status e salvar
        Task<OrderResponse> CreateOrderAsync(OrderCreateRequest request);

        // Lista todos os pedidos para a tela do React Native
        Task<IEnumerable<OrderResponse>> GetAllOrdersAsync();

        // Aprovação manual para pedidos > 5000
        Task<bool> ApproveOrderAsync(Guid orderId);
    }
}
