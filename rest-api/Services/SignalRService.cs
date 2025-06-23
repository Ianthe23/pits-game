using Microsoft.AspNetCore.SignalR;
using RestAPI.Hubs;
using RestAPI.DTOs;

namespace RestAPI.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<GameHub> _hubContext;

        public SignalRService(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyRankingsUpdate(List<GameResponse> rankings)
        {
            await _hubContext.Clients.Group("GameRoom").SendAsync("RankingsUpdated", rankings);
        }

        public async Task NotifyGameCompleted(GameResponse gameResult)
        {
            await _hubContext.Clients.Group("GameRoom").SendAsync("GameCompleted", gameResult);
        }
    }
}
