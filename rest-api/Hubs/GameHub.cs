using Microsoft.AspNetCore.SignalR;

namespace RestAPI.Hubs
{
    public class GameHub : Hub
    {
        public async Task JoinGameRoom(string playerName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "GameRoom");
            await Clients.Group("GameRoom").SendAsync("PlayerJoined", $"{playerName} has joined the game");
        }

        public async Task LeaveGameRoom(string playerName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, playerName);
            await Clients.Group("GameRoom").SendAsync("PlayerLeft", $"{playerName} has left the game");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "GameRoom");
            await base.OnDisconnectedAsync(exception);
        }

    }
}