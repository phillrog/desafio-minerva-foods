using DesafioMinervaFoods.Application.Events;
using DesafioMinervaFoods.Infrastructure.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace DesafioMinervaFoods.Infrastructure.Consumers
{
    public class OrderNotificationConsumer : IConsumer<OrderProcessedEvent>
    {
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderNotificationConsumer(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<OrderProcessedEvent> context)
        {
            var msg = context.Message;

            // LOG PARA VOCÊ VER NO TERMINAL
            Console.WriteLine("================================================");
            Console.WriteLine($"[SIGNALR] Tentando enviar notificação do Pedido: {msg.Id}");
            Console.WriteLine($"[SIGNALR] Mensagem: {msg.Message}");
            Console.WriteLine("================================================");

            try
            {
                await _hubContext.Clients.All.SendAsync("OrderNotification", new
                {
                    orderId = msg.Id,
                    userId = msg.CustomerId,
                    title = msg.Title,
                    message = msg.Message
                });

                Console.WriteLine("[SIGNALR] Sucesso: Comando enviado para o Hub.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SIGNALR] ERRO CRÍTICO: {ex.Message}");
            }
        }
    }
}
