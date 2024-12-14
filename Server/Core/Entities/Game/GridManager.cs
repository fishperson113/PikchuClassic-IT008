using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.Security.Cryptography;
namespace Server.Core.Entities.Game
{
    public class GridManager
    {
        private readonly Grid _grid;

        public GridManager(int rows, int cols)
        {
            _grid = new Grid(rows, cols);
            InitializeNodes();
        }

        private void InitializeNodes()
        {
            // Initialize Node objects and set neighbors
            Node[,] nodes = new Node[_grid.Rows + 2, _grid.Cols + 2];

            for (int i = 0; i < _grid.Rows + 2; i++)
            {
                for (int j = 0; j < _grid.Cols + 2; j++)
                {
                    if (i > 0 && i <= _grid.Rows && j > 0 && j <= _grid.Cols)
                        nodes[i, j] = new Node(i, j, _grid.Nodes[i - 1, j - 1]);
                    else
                        nodes[i, j] = new Node(i, j, 0, false); // Border nodes are not traversable
                }
            }

            SetNeighbors(nodes);
        }

        private void SetNeighbors(Node[,] nodes)
        {
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    if (nodes[i, j].IsTraversable)
                    {
                        if (i > 0 && nodes[i - 1, j].IsTraversable)
                            nodes[i, j].AddNeighbor(nodes[i - 1, j]); // Up
                        if (i < nodes.GetLength(0) - 1 && nodes[i + 1, j].IsTraversable)
                            nodes[i, j].AddNeighbor(nodes[i + 1, j]); // Down
                        if (j > 0 && nodes[i, j - 1].IsTraversable)
                            nodes[i, j].AddNeighbor(nodes[i, j - 1]); // Left
                        if (j < nodes.GetLength(1) - 1 && nodes[i, j + 1].IsTraversable)
                            nodes[i, j].AddNeighbor(nodes[i, j + 1]); // Right
                    }
                }
            }
        }

        // Implement BFS for pathfinding
        public List<Node> FindPath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            if (startNode.IsNeighbor(endNode))
            {
                path.Add(startNode);
                path.Add(endNode);
                return path;
            }

            Dictionary<Node, bool> visited = new Dictionary<Node, bool>();
            Queue<Node> queue = new Queue<Node>();
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

            queue.Enqueue(startNode);
            visited[startNode] = true;

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();

                if (current == endNode)
                {
                    Node temp = endNode;
                    while (temp != startNode)
                    {
                        path.Add(temp);
                        temp = cameFrom[temp];
                    }
                    path.Add(startNode);
                    path.Reverse();
                    return path;
                }

                foreach (var neighbor in current.Neighbors)
                {
                    if (!visited.ContainsKey(neighbor) || !visited[neighbor])
                    {
                        if (!neighbor.IsTraversable && neighbor != endNode)
                            continue;

                        visited[neighbor] = true;
                        queue.Enqueue(neighbor);
                        cameFrom[neighbor] = current;
                    }
                }
            }

            return path; // Empty path if no connection
        }

        public bool HasPath(Node startNode, Node endNode)
        {
            var path = FindPath(startNode, endNode);
            return path != null && path.Count > 0;
        }

        public void RemoveNodes(int row1, int col1, int row2, int col2)
        {
            _grid.Nodes[row1, col1] = 0;
            _grid.Nodes[row2, col2] = 0;
        }

        public Grid GetGrid()
        {
            return _grid;
        }
    }
}
