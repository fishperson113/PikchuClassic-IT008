using PikachuClassic.Infrastructure.Network.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PikachuClassic.Infrastructure.Network.DTOs
{
    public class MoveDTO
    {
        public string GameId { get; set; }
        public int PlayerId { get; set; }
        public GameMode Mode { get; set; }
        public NodeDTO FirstNode { get; set; }
        public NodeDTO SecondNode { get; set; }
    }
}
