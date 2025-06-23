namespace RestAPI.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public DateTime CreatedAt { get; set; }
        public List<Game> Games { get; set; } = new List<Game>();
    }
}