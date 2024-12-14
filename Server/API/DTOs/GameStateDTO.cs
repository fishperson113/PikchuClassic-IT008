using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.API.Enums;
namespace Server.API.DTOs
{
    public class GameStateDTO
    {
        public string GameId { get; set; }
        public int[,] Grid { get; set; }  // Mảng 2 chiều lưu ImageId của các node
        public Dictionary<int, int> Scores { get; set; }  // PlayerId -> Score
        public int CurrentTurn { get; set; }  // PlayerId của người chơi hiện tại
        public int RemainingTime { get; set; }  // Thời gian còn lại của lượt
        public GameStatus Status { get; set; }
    }
}
