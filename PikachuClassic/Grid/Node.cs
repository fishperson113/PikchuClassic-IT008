using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    public class Node
    {
        public PictureBox PictureBox { get; }
        public List<Node> Neighbors { get; }
        public bool IsBoundary { get; } // Thuộc tính để xác định node viền ngoài

        public Node(PictureBox pictureBox, bool isBoundary = false)
        {
            PictureBox = pictureBox;
            Neighbors = new List<Node>();
            IsBoundary = isBoundary; // Khởi tạo thuộc tính IsBoundary
        }

        public void AddNeighbor(Node neighbor)
        {
            Neighbors.Add(neighbor);
        }
    }
}
