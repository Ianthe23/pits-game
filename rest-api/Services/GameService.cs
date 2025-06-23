using Microsoft.EntityFrameworkCore;
using RestAPI.Data;
using RestAPI.DTOs;
using RestAPI.Models;

namespace RestAPI.Services
{
    public class GameService : IGameService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<GameService> _logger; // Fixed: Use ILogger, not ILog
        private readonly ISignalRService _signalRService;

        public GameService(GameDbContext context, ILogger<GameService> logger, ISignalRService signalRService)
        {
            _context = context;
            _logger = logger;
            _signalRService = signalRService;
        }

        public async Task<GameResponse> StartGameAsync(string playerName)
        {
            _logger.LogInformation("Starting game for player: {PlayerName}", playerName);

            var player = await _context.Players.FirstOrDefaultAsync(p => p.Name == playerName);
            if (player == null) 
            {
                throw new Exception($"Player with name {playerName} not found");
            }

            var game = new Game
            {
                PlayerId = player.Id,
                StartTime = DateTime.UtcNow,
                Points = 0,
                IsCompleted = false,
                PitElements = GeneratePitLayout(),
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Game started with ID: {GameId}", game.Id);

            return new GameResponse
            {
                GameId = game.Id,
                PlayerName = playerName,
                Points = 0,
                IsCompleted = false,
                StartTime = game.StartTime,
            };
        }

        public async Task<AttemptResponse> MakeAttemptAsync(MakeAttemptRequest request)
        {
            _logger.LogInformation("Making attempt for game ID: {GameId}, position: ({X}, {Y})", request.GameId, request.PositionX, request.PositionY);

            var game = await _context.Games.Include(g => g.PitElements)
                .FirstOrDefaultAsync(g => g.Id == request.GameId);

            if (game == null || game.IsCompleted)
            {
                _logger.LogError("Game with ID {GameId} not found or already completed", request.GameId);
                throw new InvalidOperationException($"Game with ID {request.GameId} not found or already completed");
            }

            var pitMove = game.PitElements.FirstOrDefault(p => p.PositionX == request.PositionX && p.PositionY == request.PositionY);
            
            var attempt = new GameAttempt
            {
                GameId = game.Id,
                PositionX = request.PositionX,
                PositionY = request.PositionY,
                IsPitElement = pitMove != null,
                IsWinningMove = pitMove == null && request.PositionX == 3,
            };

            _context.GameAttempts.Add(attempt);

            if (pitMove != null)
            {
                game.Points += request.PositionX;
            }

            _logger.LogInformation("Attempt made: {AttemptId}, isPitElement: {IsPitElement}, isWinningMove: {IsWinningMove}", attempt.Id, attempt.IsPitElement, attempt.IsWinningMove);
            
            bool isFinishingMove = pitMove != null || request.PositionX == 3;
            if (isFinishingMove)
            {
                game.IsCompleted = true;
                game.EndTime = DateTime.UtcNow;

                _logger.LogInformation("Game completed with ID: {GameId}", game.Id);
            }

            await _context.SaveChangesAsync();

            var response = new AttemptResponse
            {
                IsFinishingMove = isFinishingMove,
                Points = game.Points,
                IsGameCompleted = game.IsCompleted,
            };

            if (game.IsCompleted)
            {
                response.GameResult = new GameResponse
                {
                    GameId = game.Id,
                    PlayerName = game.Player.Name,
                    Points = game.Points,
                    IsCompleted = game.IsCompleted,
                    StartTime = game.StartTime,
                    EndTime = game.EndTime,
                    PitElements = game.PitElements.Select(p => new PitElement
                    {
                        PositionX = p.PositionX,
                        PositionY = p.PositionY,
                    }).ToList(),
                };

                await _signalRService.NotifyGameCompleted(response.GameResult);
                var updatedRanking = await GetRankingAsync();
                await _signalRService.NotifyRankingsUpdate(updatedRanking);
            };

            return response;
        }

        public async Task<List<GameResponse>> GetRankingAsync()
        {
            var games = await _context.Games
                .Include(g => g.Player)
                .Where(g => g.IsCompleted)
                .OrderByDescending(g => g.Points)
                .Take(10)
                .ToListAsync();

            return games.Select(g => new GameResponse
            {
                GameId = g.Id,
                PlayerName = g.Player.Name,
                Points = g.Points,
                IsCompleted = g.IsCompleted,
                StartTime = g.StartTime,
                EndTime = g.EndTime,
                DurationSeconds = (int)(g.EndTime?.Subtract(g.StartTime).TotalSeconds ?? 0),
            }).ToList();
        }

        private List<PitElement> GeneratePitLayout()
        {
            var pits = new List<PitElement>();
    
            for (int row = 0; row < 4; row++)
            {
                int pitsInRow = Random.Shared.Next(1, 4);
                for (int i = 0; i < pitsInRow; i++)
                {
                    int column = Random.Shared.Next(0, 4);
                    pits.Add(new PitElement
                    {
                        PositionX = row,
                        PositionY = column,
                    });
                }
            }
            
            return pits;
        }
    }
}