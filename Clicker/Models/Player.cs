using Clicker.Enums;

namespace Clicker.Models
{
    public class Player
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public double AccumulatedTime { get; set; } = 0;
        public PlayerStatus Status { get; set; } = PlayerStatus.Active;
    }
}
