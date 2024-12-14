using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PikachuClassic.Infrastructure.Network.Enums;
namespace PikachuClassic.Infrastructure.Network.DTOs
{
    public class GameStateDTO
    {
        public string GameId { get; set; }
        public int[,] Grid { get; set; }
        public Dictionary<int, int> Scores { get; set; }
        public int CurrentTurn { get; set; }
        public int RemainingTime { get; set; }
        public GameStatus Status { get; set; }
    }
}
