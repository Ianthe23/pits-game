namespace RestAPI.Models
{
    public class GameAttempt
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public bool IsPitElement{ get; set; }
        public bool IsWinningMove { get; set; }
    }
}