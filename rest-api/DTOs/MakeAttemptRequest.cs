namespace RestAPI.DTOs
{
    public class MakeAttemptRequest
    {
        public int GameId { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
}