using Microsoft.AspNetCore.SignalR;

namespace DesafioMinervaFoods.Infrastructure.Hubs
{
    public class OrderHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"--> ALGUÉM CONECTOU NO HUB! ConnectionId: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }
    }
}
