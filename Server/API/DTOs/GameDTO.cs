using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.API.Enums;
namespace Server.API.DTOs
{
    public class GameDTO
    {
        public string GameId { get; set; }
        public GameMode Mode { get; set; }
        public int[,] Grid { get; set; }
        public List<PlayerDTO> Players { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
