using RestAPI.DTOs;

namespace RestAPI.Services
{
    public interface IGameService
    {
        Task<GameResponse> StartGameAsync(string playerName);
        Task<AttemptResponse> MakeAttemptAsync(MakeAttemptRequest request);
        Task<List<GameResponse>> GetRankingAsync();
    }
}