using Microsoft.AspNetCore.Mvc;
using RestAPI.Services;
using RestAPI.DTOs;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }
        

        [HttpPost("start")]
        public async Task<IActionResult> StartGame([FromBody] StartGameRequest request)
        {
            try
            {
                var game = await _gameService.StartGameAsync(request.PlayerName);
                return Ok(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting game for player: {PlayerName}", request.PlayerName);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("attempt")]
        public async Task<IActionResult> MakeAttempt([FromBody] MakeAttemptRequest request)
        {
            try
            {
                var attempt = await _gameService.MakeAttemptAsync(request);
                return Ok(attempt);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error making attempt for game ID: {GameId}, position: ({X}, {Y})", request.GameId, request.PositionX, request.PositionY);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("rankings")]
        public async Task<IActionResult> GetRankings()
        {
            try
            {
                var rankings = await _gameService.GetRankingAsync();
                return Ok(rankings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ranking");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}