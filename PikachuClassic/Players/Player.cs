using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikachuClassic
{
    public class Player
    {
        protected int score=0;
        protected string name;
        public Player()
        {
            
        }
        public void UpdateScore(int score)
        {
            this.score += score;
        }
        public int Score
        {
            get
            {
                return score;
            }
        }
    }
}
