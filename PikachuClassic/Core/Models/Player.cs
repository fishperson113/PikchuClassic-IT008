using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikachuClassic.Core.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public Player() { }

        public Player(int id, string name)
        {
            Id = id;
            Name = name;
            Score = 0;
        }
    }
}
