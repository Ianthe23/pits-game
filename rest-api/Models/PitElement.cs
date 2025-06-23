namespace RestAPI.Models 
{
    public class PitElement
    {
        public int Id { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}