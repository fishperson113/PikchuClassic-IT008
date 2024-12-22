using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    public class Node
    {
        // hệ trục tọa độ tạm hiểu ở đây:
        // tia x gióng xuống
        // tia y gióng sang phải
        public int X { get; set; }
        public int Y { get; set; }
        public PictureBox pictureBox { get; set; }
        private List<Node> originalNeighbors;
        public List<Node> Neighbors { get; }
        public bool isTraversable { get; set; }

        public Node(PictureBox pictureBox, bool IsTraversable, int x, int y)
        {
            this.pictureBox = pictureBox;
            Neighbors = new List<Node>();
            originalNeighbors = new List<Node>();
            isTraversable = IsTraversable;
            X = x;
            Y = y;
        }

        public void AddNeighbor(Node neighbor)
        {
            Neighbors.Add(neighbor);
            originalNeighbors.Add(neighbor);
        }
        public void ApplyPermutation(int[] permutation)
        {
            Neighbors.Clear();
            foreach (int index in permutation)
            {
                if (index < originalNeighbors.Count)
                {
                    Neighbors.Add(originalNeighbors[index]);
                }
            }
        }
        public bool isNeighbor(Node neighbor)
        {
            return Neighbors.Contains(neighbor);
        }
        public void ResetNeighbors()
        {
            Neighbors.Clear();
            Neighbors.AddRange(originalNeighbors);
        }

    }
}
