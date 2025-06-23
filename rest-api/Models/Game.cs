namespace RestAPI.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Points { get; set; }
        public bool IsCompleted { get; set; }
        public List<PitElement> PitElements { get; set; } = new List<PitElement>();
        public List<GameAttempt> Attempts { get; set; } = new List<GameAttempt>();
    }
}