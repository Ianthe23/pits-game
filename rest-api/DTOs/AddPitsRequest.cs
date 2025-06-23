namespace RestAPI.DTOs
{
    public class AddPitsRequest
    {
        public List<PitPosition> Pits { get; set; } = new List<PitPosition>();
    }

    public class PitPosition
    {
        public int PositionX { get; set; }
        public int PositionY { get; set; }
    }
}