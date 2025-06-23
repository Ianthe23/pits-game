using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI.Data;
using RestAPI.Models;
using RestAPI.Services;
using RestAPI.DTOs;

namespace RestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly GameDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(GameDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("pits")]
        public async Task<ActionResult<List<PitElement>>> GetPits()
        {
            var pits = await _context.PitElements.ToListAsync();
            return Ok(pits);
        }

        [HttpGet("games/{gameId}")]
        public async Task<ActionResult<Game>> GetGame(int gameId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
                return NotFound();
            return Ok(game);
        }

        // Add pit elements to an existing game
        [HttpPost("games/{gameId}/pits")]
        public async Task<ActionResult<List<PitPosition>>> AddPitsToGame(
            int gameId, 
            [FromBody] AddPitsRequest request)
        {
            try
            {
                // Find the game (must exist)
                var game = await _context.Games
                    .Include(g => g.PitElements)  // Load existing pits
                    .FirstOrDefaultAsync(g => g.Id == gameId);

                if (game == null)
                    return NotFound($"Game with ID {gameId} not found");

                if (game.IsCompleted)
                    return BadRequest("Cannot add pits to a completed game");

                // Validate new pit positions
                var newPits = new List<PitElement>();
                foreach (var pitRequest in request.Pits)
                {
                    // Check if position is valid (0-3 for 4x4 grid)
                    if (pitRequest.PositionX < 0 || pitRequest.PositionX> 3 || 
                        pitRequest.PositionY < 0 || pitRequest.PositionY > 3)
                    {
                        return BadRequest($"Invalid position: ({pitRequest.PositionX}, {pitRequest.PositionY})");
                    }

                    // Check if pit already exists at this position
                    var existingPit = game.PitElements.Any(p => 
                        p.PositionX == pitRequest.PositionX && p.PositionY == pitRequest.PositionY);
                    
                    if (existingPit)
                    {
                        return BadRequest($"Pit already exists at position ({pitRequest.PositionX}, {pitRequest.PositionY})");
                    }

                    // Create new pit element
                    var newPit = new PitElement
                    {
                        GameId = gameId,  // Set the foreign key
                        PositionX = pitRequest.PositionX,
                        PositionY = pitRequest.PositionY,
                    };

                    newPits.Add(newPit);
                }

                // Add new pits to the game
                game.PitElements.AddRange(newPits);
                // OR add directly to context:
                // _context.PitElements.AddRange(newPits);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Added {Count} pits to game {GameId}", newPits.Count, gameId);

                // Return the newly created pits
                var pitDtos = newPits.Select(p => new PitPosition
                {
                    PositionX = p.PositionX,
                    PositionY = p.PositionY,
                }).ToList();

                return Ok(pitDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding pits to game {GameId}", gameId);
                return StatusCode(500, "An error occurred while adding pits");
            }
        }
    }
}