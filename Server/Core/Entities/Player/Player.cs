using Server.API.DTOs;
using Server.Core.Entities.Game;
namespace Server.Core.Entities.Player
{
    public class Player
    {
        public int PlayerId { get; private set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public Player(int id, string name = "Player")
        {
            PlayerId = id;
            Name = name;
            Score = 0;
        }

        public virtual MoveDTO MakeMove(GameStateDTO gameState, Grid grid)
        {
            // Real players make moves via client, so server does not handle this
            return null;
        }
    }
}
