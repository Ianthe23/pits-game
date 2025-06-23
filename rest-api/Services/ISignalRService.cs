using RestAPI.DTOs;

namespace RestAPI.Services
{
    public interface ISignalRService
    {
        Task NotifyRankingsUpdate(List<GameResponse> rankings);
        Task NotifyGameCompleted(GameResponse game);
    }
}