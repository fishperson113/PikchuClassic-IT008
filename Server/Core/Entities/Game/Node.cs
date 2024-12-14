using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Entities.Game
{
    public class Node
    {
        public int X { get; set; } // Row
        public int Y { get; set; } // Column
        public int ImageId { get; set; }
        public List<Node> Neighbors { get; } = new List<Node>();
        public bool IsTraversable { get; set; }

        public Node(int x, int y, int imageId, bool isTraversable = true)
        {
            X = x;
            Y = y;
            ImageId = imageId;
            IsTraversable = isTraversable;
        }

        public void AddNeighbor(Node neighbor)
        {
            Neighbors.Add(neighbor);
        }

        public bool IsNeighbor(Node neighbor)
        {
            return Neighbors.Contains(neighbor);
        }
        public override bool Equals(object obj)
        {
            if (obj is Node other)
            {
                return this.X == other.X && this.Y == other.Y && this.ImageId == other.ImageId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, ImageId);
        }
    }
}
