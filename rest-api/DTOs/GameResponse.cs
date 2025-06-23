using RestAPI.Models;

namespace RestAPI.DTOs
{
    public class GameResponse
    {
        public int GameId { get; set; }
        public string PlayerName { get; set; }
        public int Points { get; set; }
        public bool IsCompleted { get; set; }
        public List<PitElement> PitElements { get; set; } = new List<PitElement>();
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationSeconds { get; set; }
    }
}