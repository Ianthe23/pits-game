namespace RestAPI.DTOs
{
    public class AttemptResponse
    {
        public bool IsFinishingMove { get; set; }
        public int Points { get; set; }
        public bool IsGameCompleted { get; set; }
        public GameResponse? GameResult { get; set; }
    }
}