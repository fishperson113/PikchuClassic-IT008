using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    public enum ScoreGroup
    {
        Group10 = 10,
        Group20 = 20,
        Group30 = 30,
        Group40 = 40,
        Group50 = 50,
        Group60 = 60,
    }
    public class Grid
    {
        #region Properties
        // Thuộc tính của grid
        private int rows;
        private int cols;
        private PictureBox[,] pictureGrid;
        private Panel gridPanel;
        private int cellSize;
        private List<Image> imagesList = new List<Image>(); // Danh sách các cặp hình ảnh sẽ được gán vào các ô 
        private Node[,] nodes; // Bảo thêm, khai báo mảng 2 chiều các node đồ thị

        // Danh sách các cặp hình ảnh và nhóm điểm tương ứng
        private static List<Image> allImages = new List<Image>(); // Danh sách tất cả các hình ảnh có sẵn trong resources
        private static Dictionary<Image, ScoreGroup> imageScoreGroups = new Dictionary<Image, ScoreGroup>();
        private static ScoreGroup[] scoreGroups = (ScoreGroup[])Enum.GetValues(typeof(ScoreGroup));
        private static bool isScoreGroupsAssigned = false;
        #endregion
        public Grid(Panel panel, int rows, int cols)
        {
            this.gridPanel = panel;
            this.rows = rows;
            this.cols = cols;
            this.pictureGrid = new PictureBox[rows + 2, cols + 2]; // 1 - Bảo thêm + 2
            
            // Dương, đoạn này để DoubleBuffer
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, gridPanel, new object[] { true });
            
            CalculateCellSize();
            gridPanel.BackgroundImageLayout = ImageLayout.Stretch;
            if (!isScoreGroupsAssigned)
            {
                LoadResource(); // Load tất cả hình ảnh từ resources (tên file: _0, _1, _2, ...
                AssignScoreGroups();
                isScoreGroupsAssigned = true; // Đánh dấu là đã gán nhóm điểm
            }

        }
        private void CalculateCellSize()
        {
            int availableWidth = gridPanel.Width;
            int availableHeight = gridPanel.Height;

            // Kích thước của ô là kích thước nhỏ nhất giữa chiều rộng và chiều cao có sẵn
            cellSize = Math.Min(availableWidth / (cols + 2), availableHeight / (rows + 2)); // 3 - Bảo thêm + 2
        }
        public void GenerateGrid()
        {
            int totalGridWidth = (cols + 2) * cellSize; // 2 - Bảo thêm + 2
            int totalGridHeight = (rows + 2) * cellSize;

            int offsetX = (gridPanel.Width - totalGridWidth) / 2; // Căn giữa theo chiều ngang
            int offsetY = (gridPanel.Height - totalGridHeight) / 2; // Căn giữa theo chiều dọc

            for (int i = 0; i < (rows + 2); i++) // 4 - Bảo thêm + 2 vào đây
            {
                for (int j = 0; j < (cols + 2); j++) // 4 - Bảo thêm + 2 vào đây
                {
                    // Tạo PictureBox cho từng ô
                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Width = cellSize;
                    pictureBox.Height = cellSize;
                    pictureBox.BorderStyle = BorderStyle.FixedSingle;
                    pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    // Đặt vị trí cho PictureBox trong Panel
                    pictureBox.Location = new Point
                    (
                        offsetX + j * cellSize,
                        offsetY + i * cellSize
                    );

                    // Đặt PictureBox hiển thị phía trên background
                    pictureBox.BackColor = Color.Transparent;

                    // Thêm PictureBox vào Panel và grid
                    gridPanel.Controls.Add(pictureBox);
                    pictureGrid[i, j] = pictureBox;

                }
            }
            
            AssignImagesToGrid();

            // Bảo thêm
            nodes = new Node[rows + 2, cols + 2];
            InitializeNodes();
            SetNeighbors();
            // Hết Bảo

            // Debug isTraversable
            foreach (var node in nodes)
            {
                Console.Write($"Node [{node.X}, {node.Y}]: ");
                Console.WriteLine(node.isTraversable);
            }
        }
        private void LoadResource()
        {
            for (int i = 0; ; i++)
            {
                Image img = (Image)Properties.Resources.ResourceManager.GetObject($"_{i}");
                if (img == null) break;
                allImages.Add(img);
            }
        }
        private void AssignImagesToGrid()
        {
            //if (imagesList != null) return;
            int totalCells = rows * cols; // hiểu là totalInnerCells (Bảo)
            if (totalCells % 2 != 0)
            {
                MessageBox.Show("Số lượng ô phải là số chẵn để có thể gán đủ các cặp hình!");
                return;
            }
            Shuffle(allImages);

            /*
            for (int i = 0; i < totalCells / 2; i++) // Chia đôi vì mỗi ảnh xuất hiện 2 lần
            {
                Image img = allImages[i % allImages.Count]; // i% allImages.Count để đảm bảo totalCells/2 < allImages.Count (tránh lỗi index out of range) nếu vượt quá thì nó quay về index=0
                imagesList.Add(img); // Thêm lần 1
                imagesList.Add(img); // Thêm lần 2 (tạo cặp)
            }

            Shuffle(imagesList);
            */

            // Bảo, tạo ra tempImageList để fix cứng cặp đầu
            List<Image> tempImagesList = new List<Image>();
            for (int i = 1; i < (totalCells / 2); i++) // i = 1 để ko xáo cặp đầu
            {
                if (i % allImages.Count == 0)
                    continue;

                Image img = allImages[i % allImages.Count]; // Dương, i% allImages.Count để đảm bảo totalCells/2 < allImages.Count (tránh lỗi index out of range) nếu vượt quá thì nó quay về index=0
                tempImagesList.Add(img);
                tempImagesList.Add(img);
            }
            Shuffle(tempImagesList);

            for (int i = 0; i < totalCells; i++)
            {
                if (i < 2)
                    imagesList.Add(allImages[0]);
                else
                    imagesList.Add(tempImagesList[i - 2]);
            }

            // Gán hình ảnh từ danh sách đã xáo trộn vào các PictureBox trong grid
            int imageIndex = 0;
            for (int i = 1; i < rows + 1; i++) // 5 - Bảo thay đổi chỉ số vòng for
            {
                for (int j = 1; j < cols + 1; j++) // 5 - Nguyên bản: [i/j = 0], [< rows/cols]
                {
                    pictureGrid[i, j].Image = imagesList[imageIndex];
                    imageIndex++;
                }
            }
        }
        private void AssignScoreGroups()
        {
            int imagesPerGroup = 6; // Số lượng hình ảnh mỗi nhóm
            int currentGroupIndex = 0;

            for (int i = 0; i < allImages.Count; i++)
            {
                Image img = allImages[i];
                ScoreGroup group = scoreGroups[currentGroupIndex];

                // Gán nhóm điểm cho ảnh
                imageScoreGroups[img] = group;
                Console.WriteLine($"Ảnh: {i} - Nhóm điểm: {group}");

                // Tăng nhóm khi đạt số lượng yêu cầu
                if ((i + 1) % imagesPerGroup == 0 && currentGroupIndex < scoreGroups.Length - 1)
                {
                    currentGroupIndex++;
                }
            }
        }

        // Hàm xáo trộn danh sách (Fisher-Yates shuffle)
        private void Shuffle(List<Image> images)
        {
            Random rng = new Random();
            int n = images.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Image value = images[k]; // Swap
                images[k] = images[n];
                images[n] = value;
            }
        }

        //Bảo, khởi tạo các node
        private void InitializeNodes() // 6 - Sửa đổi lại hàm
        {
            // Khởi tạo node
            for (int i = 0; i < rows + 2; i++) // 6.1 - Bảo thay bằng rows + 2. Nguyên bản: getLength(0)
            {
                for (int j = 0; j < cols + 2; j++) // 6.1
                {
                    // 6.2 - Do pictureGrid đã có kích cỡ bằng với nodes[,] nên phải gán lại các picBox tương ứng
                    if (i == 0 || j == 0 || i == (rows + 1) || j == (cols + 1))
                    {
                        nodes[i, j] = new Node(pictureGrid[i, j], true, i, j); // Viền ngoài
                        nodes[i, j].pictureBox.Visible = false;
                    }    
                    else
                    {
                        nodes[i, j] = new Node(pictureGrid[i, j], false, i, j); // Bên trong
                        nodes[i, j].pictureBox.Visible = true;
                    }
                }
            }
        }

        //Bảo, khởi tạo danh sách Neighbors cho node
        private void SetNeighbors()
        {
            for (int i = 0; i < (rows + 2); i++)
            {
                for (int j = 0; j < (cols + 2); j++)
                {
                    if (i > 0 && nodes[i - 1, j] != null)
                        nodes[i, j].AddNeighbor(nodes[i - 1, j]); // Trên
                    if (i < (rows + 1) && nodes[i + 1, j] != null)
                        nodes[i, j].AddNeighbor(nodes[i + 1, j]); // Dưới
                    if (j > 0 && nodes[i, j - 1] != null)
                        nodes[i, j].AddNeighbor(nodes[i, j - 1]); // Trái
                    if (j < (cols + 1) && nodes[i, j + 1] != null)
                        nodes[i, j].AddNeighbor(nodes[i, j + 1]); // Phải
                }
            }
        }

        public PictureBox[,] GetPictureBoxes()
        {
            return pictureGrid;
        }
        
        public bool AllPictureBoxesHidden() // Dương
        {
            // Duyệt qua tất cả các PictureBox trong grid
            for (int i = 0; i < (rows + 2); i++) // 8 - Bảo thêm + 2
            {
                for (int j = 0; j < (cols + 2); j++)
                {
                    // Kiểm tra nếu PictureBox vẫn còn hiển thị
                    if (pictureGrid[i, j].Visible)
                    {
                        return false; // Nếu có bất kỳ PictureBox nào vẫn hiển thị, trả về false
                    }
                }
            }
            return true; // Nếu tất cả đều ẩn, trả về true
        }
        public ScoreGroup GetScoreForImage(Image image) // Dương
        {
            if (imageScoreGroups.TryGetValue(image, out ScoreGroup score))
            {
                Console.WriteLine($"Điểm của ảnh: {score}");
                return score;
            }
            Console.WriteLine("Không tìm thấy ảnh trong dictionary");
            return ScoreGroup.Group10; // Điểm mặc định nếu không tìm thấy ảnh
        }

        // Bảo, dùng để loại node đã bị vô hiệu hóa khi chọn 2 hình giống nhau
        internal void RemoveNodes(PictureBox firstBox, PictureBox secondBox)
        {
            Node firstNode = GetNodeFromPictureBox(firstBox);
            Node secondNode = GetNodeFromPictureBox(secondBox);

            firstNode.isTraversable = true;
            secondNode.isTraversable = true;
        }

        //Bảo, lấy ánh xạ từ vị trí của PictureBox -> vị trí của node
        public Node GetNodeFromPictureBox(PictureBox pictureBox)
        {
            for (int i = 0; i < (rows + 2); i++)
            {
                for (int j = 0; j < (cols + 2); j++)
                {
                    if (pictureGrid[i, j] == pictureBox)
                        return nodes[i, j]; // 7 - Bảo thay đổi tọa độ ánh xạ (do pictureGrid đã thay đổi)
                }
            }
            return null; // Trả về null nếu không tìm thấy
        }

        /*
        //Bảo, hàm này để tìm đường đi và trả về danh sách các node đã đi qua
        public List<Node> FindPath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            if (startNode.isNeighbor(endNode)) // Nếu 2 node là hàng xóm
            {
                path.Add(startNode);
                path.Add(endNode);
                return path;
            }

            Dictionary<Node, bool> visited = new Dictionary<Node, bool>();
            Queue<Node> queue = new Queue<Node>();
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            Dictionary<Node, Direction> directionFrom = new Dictionary<Node, Direction>();

            queue.Enqueue(startNode);
            visited[startNode] = true;

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();

                if (current == endNode) // Nếu đã tìm được đường đi
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
                    if (visited.ContainsKey(neighbor) && visited[neighbor])
                        continue;
                    if (!neighbor.isTraversable && neighbor != endNode)
                        continue;
                    visited[neighbor] = true;
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current;


                    Direction direction = GetDirection(current, neighbor);
                    if (directionFrom.ContainsKey(current) && directionFrom[current] != direction) // Vừa rẽ hướng
                    {
                        if (pathChangesExceeded(cameFrom, neighbor, 2))
                        {
                            visited[neighbor] = false;
                            queue = new Queue<Node>(queue.Where(n => n != neighbor)); // Loại bỏ node
                            continue;
                        }
                    }
                    directionFrom[neighbor] = direction;

                }
            }

            return null; // Không tìm thấy đường đi
        }
        

        private Direction GetDirection(Node from, Node to)
        {
            if (to.X > from.X) return Direction.Down;
            if (to.X < from.X) return Direction.Up;
            if (to.Y > from.Y) return Direction.Right;
            return Direction.Left;
        }

        
        private bool pathChangesExceeded(Dictionary<Node, Node> cameFrom, Node node, int maxChanges)
        {
            int changes = 0;
            Node temp = node; // ý tưởng: temp = neighbor (ý tưởng lúc truyền tham số)
            while (cameFrom.ContainsKey(temp))
            {
                Direction currentDirection = GetDirection(temp, cameFrom[temp]);
                temp = cameFrom[temp];

                if (cameFrom.ContainsKey(temp) && currentDirection != GetDirection(temp, cameFrom[temp]))
                {
                    changes++;
                    if (changes > maxChanges)
                        return true;
                }
            }
            return false;
        }
        

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        */

        /*
        public List<Node> FindPathDijkstra(Node startNode, Node endNode)
        {
            if (startNode == null || endNode == null)
                return null;

            // Dictionary để lưu khoảng cách ngắn nhất đến từng node
            Dictionary<Node, int> distances = new Dictionary<Node, int>();
            // Dictionary để lưu node cha (truy vết đường đi)
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            // Dictionary để lưu số lần rẽ hướng
            Dictionary<Node, int> bends = new Dictionary<Node, int>();
            // Priority Queue
            SortedSet<(int distance, Node node)> priorityQueue = new SortedSet<(int distance, Node node)>();

            // Khởi tạo
            foreach (var neighbor in GetAllNodes())
            {
                distances[neighbor] = int.MaxValue;
                bends[neighbor] = int.MaxValue;
            }
            distances[startNode] = 0;
            bends[startNode] = 0;
            priorityQueue.Add((0, startNode));

            // Dijkstra
            while (priorityQueue.Count > 0)
            {
                var (currentDistance, currentNode) = priorityQueue.Min;
                priorityQueue.Remove(priorityQueue.Min);

                if (currentNode == endNode)
                    break;

                foreach (var neighbor in currentNode.Neighbors)
                {
                    if (!neighbor.isTraversable && neighbor != endNode)
                        continue;

                    int newDistance = currentDistance + 1;
                    int newBends = bends[currentNode];
                    if (cameFrom.ContainsKey(currentNode) && GetDirection(cameFrom[currentNode], currentNode) != GetDirection(currentNode, neighbor))
                    {
                        newBends++;
                    }

                    if (newDistance < distances[neighbor] && newBends <= 2)
                    {
                        priorityQueue.Remove((distances[neighbor], neighbor)); // Loại node cũ khỏi hàng đợi
                        distances[neighbor] = newDistance;
                        bends[neighbor] = newBends;
                        cameFrom[neighbor] = currentNode;
                        priorityQueue.Add((newDistance, neighbor));
                    }
                }
            }

            // Truy ngược đường đi
            if (!cameFrom.ContainsKey(endNode))
                return null;

            List<Node> path = new List<Node>();
            Node temp = endNode;
            while (temp != null)
            {
                path.Add(temp);
                cameFrom.TryGetValue(temp, out temp);
            }
            path.Reverse();
            return path;
        }

        // Lấy hướng di chuyển giữa hai node
        private Direction GetDirection(Node from, Node to)
        {
            if (to.X > from.X) return Direction.Down;
            if (to.X < from.X) return Direction.Up;
            if (to.Y > from.Y) return Direction.Right;
            return Direction.Left;
        }

        // Enum để biểu diễn hướng di chuyển
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        // Lấy tất cả các node trong đồ thị
        private List<Node> GetAllNodes()
        {
            List<Node> allNodes = new List<Node>();
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    if (nodes[i, j] != null)
                        allNodes.Add(nodes[i, j]);
                }
            }
            return allNodes;
        }
        */

        
        public List<Node> FindPath(Node firstNode, Node secondNode)
        {
            Console.WriteLine("Create list path");
            List<Node> path = new List<Node>();

            Console.WriteLine("firstNode + secondNode isTraversable");
            firstNode.isTraversable = true;
            secondNode.isTraversable = true;

            // Check I-shape
            Console.WriteLine("Entering 1st condition check:");
            if (firstNode.X == secondNode.X && Check_Ishape_X(firstNode.Y, secondNode.Y, firstNode.X))
            {
                path.Add(firstNode);
                path.Add(secondNode);
                
                Console.WriteLine("1st condition clear");
                return path;
            }
            Console.WriteLine("1st condition fail");

            Console.WriteLine("Entering 2nd condition check:");
            if (firstNode.Y == secondNode.Y && Check_Ishape_Y(firstNode.X, secondNode.X, firstNode.Y))
            {
                path.Add(firstNode);
                path.Add(secondNode);
                
                Console.WriteLine("2nd condition clear");
                return path;
            }
            Console.WriteLine("2nd condition fail");

            // Check L-shape
            Console.WriteLine("Entering 3rd condition check:");
            if (Check_Lshape_X(firstNode, secondNode))
            {
                path.Add(firstNode);
                path.Add(nodes[firstNode.X, secondNode.Y]);
                path.Add(secondNode);
                
                Console.WriteLine("3rd condition clear");
                return path;
            }
            Console.WriteLine("3rd condition fail");

            Console.WriteLine("Entering 4th condition check:");
            if (Check_Lshape_Y(firstNode, secondNode))
            {
                path.Add(firstNode);
                path.Add(nodes[secondNode.X, firstNode.Y]);
                path.Add(secondNode);
                
                Console.WriteLine("4th condition clear");
                return path;
            }
            Console.WriteLine("4th condition fail");

            // Check N-shape, U-shape
            int t; // để thử ko gán -1 ở đây xem sao

            // Check N-shape
            Console.WriteLine("Entering 5th condition check:");
            if ((t = Check_Nshape_X(firstNode, secondNode)) != -1)
            {
                path.Add(firstNode);
                path.Add(nodes[firstNode.X, t]);
                path.Add(nodes[secondNode.X, t]);
                path.Add(secondNode);

                Console.WriteLine("5th condition clear");
                return path;
            }
            Console.WriteLine("5th condition fail");

            Console.WriteLine("Entering 6th condition check:");
            if ((t = Check_Nshape_Y(firstNode, secondNode)) != -1)
            {
                path.Add(firstNode);
                path.Add(nodes[t, firstNode.Y]);
                path.Add(nodes[t, secondNode.Y]);  
                path.Add(secondNode);
                
                Console.WriteLine("6th condition clear");
                return path;
            }
            Console.WriteLine("6th condition fail");

            // Check U-shape
            Console.WriteLine("Entering 7th condition check:");
            if ((t = Check_Ushape_X(firstNode, secondNode, 1)) != -1)
            {
                path.Add(firstNode);
                path.Add(nodes[firstNode.X, t]);
                path.Add(nodes[secondNode.X, t]);
                path.Add(secondNode);

                Console.WriteLine("7th condition clear");
                return path;
            }
            Console.WriteLine("7th condition fail");
            
            Console.WriteLine("Entering 8th condition check:");
            if ((t = Check_Ushape_X(firstNode, secondNode, -1)) != -1)
            {
                path.Add(firstNode);
                path.Add(nodes[firstNode.X, t]);
                path.Add(nodes[secondNode.X, t]);
                path.Add(secondNode);

                Console.WriteLine("8th condition clear");
                return path;
            }
            Console.WriteLine("8th condition fail");
            
            Console.WriteLine("Entering 9th condition check:");
            if ((t = Check_Ushape_Y(firstNode, secondNode, 1)) != -1)
            {
                path.Add(firstNode);
                path.Add(nodes[t, firstNode.Y]);
                path.Add(nodes[t, secondNode.Y]);
                path.Add(secondNode);
                
                Console.WriteLine("9th condition clear");
                return path;
            }
            Console.WriteLine("9th condition fail");
            
            Console.WriteLine("Entering 10th condition check:");
            if ((t = Check_Ushape_Y(firstNode, secondNode, -1)) != -1)
            {
                path.Add(firstNode);
                path.Add(nodes[t, firstNode.Y]);
                path.Add(nodes[t, secondNode.Y]);
                path.Add(secondNode);
                
                Console.WriteLine("10th condition clear");
                return path;
            }
            Console.WriteLine("10th condition fail");

            Console.WriteLine("firstNode + secondNode is not Traversable");
            firstNode.isTraversable = false;
            secondNode.isTraversable = false;
            return null;
        }

        // Kiểm tra dòng ngang từ cột y1 đến y2 tại hàng x
        private bool Check_Ishape_X(int y1, int y2, int x)
        {
            int min = Math.Min(y1, y2);
            int max = Math.Max(y1, y2);

            for (int y = min; y <= max; y++)
            {
                if (nodes[x, y].isTraversable == false) // Nếu gặp vật cản, trả về false
                    return false;
            }
            return true; // Không gặp vật cản, trả về true
        }

        // Kiểm tra dòng dọc từ hàng x1 đến x2 tại cột y
        private bool Check_Ishape_Y(int x1, int x2, int y)
        {
            int min = Math.Min(x1, x2);
            int max = Math.Max(x1, x2);

            for (int x = (min); x <= max; x++)
            {
                if (nodes[x, y].isTraversable == false)
                    return false;
            }
            return true;
        }

        // Kiểm tra đường đi hình chữ L theo hàng X
        private bool Check_Lshape_X(Node n1, Node n2)
        {
            if (Check_Ishape_X(n1.Y, n2.Y, n1.X) &&
                Check_Ishape_Y(n1.X, n2.X, n2.Y))
                return true;
            return false;
        }

        // Kiểm tra đường đi hình chữ L theo cột Y
        private bool Check_Lshape_Y(Node n1, Node n2)
        {
            if (Check_Ishape_Y(n1.X, n2.X, n1.Y) &&
                Check_Ishape_X(n1.Y, n2.Y, n2.X))
                return true;
            return false;
        }

        // Kiểm tra hình chữ nhật bằng cách chia theo trục X
        private int Check_Nshape_X(Node n1, Node n2)
        {
            int nMinY_Y = n1.Y, nMaxY_Y = n2.Y;
            int nMinY_X = n1.X, nMaxY_X = n2.X;

            if (n1.Y > n2.Y)
            {
                nMinY_Y = n2.Y; nMaxY_Y = n1.Y;
                nMinY_X = n2.X; nMaxY_X = n1.X;
            }

            for (int y = nMinY_Y + 1; y < nMaxY_Y; y++)
            {
                if (Check_Ishape_X(nMinY_Y, y, nMinY_X) &&
                    Check_Ishape_Y(nMinY_X, nMaxY_X, y) &&
                    Check_Ishape_X(y, nMaxY_Y, nMaxY_X))
                    return y;
            }
            return -1;
        }

        // Kiểm tra hình chữ nhật bằng cách chia theo trục Y
        private int Check_Nshape_Y(Node n1, Node n2)
        {
            int nMinX_X = n1.X, nMaxX_X = n2.X;
            int nMinX_Y = n1.Y, nMaxX_Y = n2.Y;

            if (n1.X > n2.X)
            {
                nMinX_X = n2.X; nMaxX_X = n1.X;
                nMinX_Y = n2.Y; nMaxX_Y = n1.Y;
            }

            for (int x = nMinX_X + 1; x < nMaxX_X; x++)
            {
                if (Check_Ishape_Y(nMinX_X, x, nMinX_Y) &&
                    Check_Ishape_X(nMinX_Y, nMaxX_Y, x) &&
                    Check_Ishape_Y(x, nMaxX_X, nMaxX_Y))
                    return x;
            }
            return -1;
        }

        // Kiểm tra thêm các đường ngang
        private int Check_Ushape_X(Node n1, Node n2, int type)
        {
            int nMinY_Y = n1.Y, nMaxY_Y = n2.Y;
            int nMinY_X = n1.X, nMaxY_X = n2.X;

            if (n1.Y > n2.Y)
            {
                nMinY_Y = n2.Y; nMaxY_Y = n1.Y;
                nMinY_X = n2.X; nMaxY_X = n1.X;
            }

            int y = (type == -1) ? (nMinY_Y - 1) : (nMaxY_Y + 1);
            int row = (type == -1) ? nMaxY_X : nMinY_X;

            if (Check_Ishape_X(nMinY_Y, nMaxY_Y, row))
            {
                while (y >= 0 && y <= (cols + 1)) // Để ko vượt ra ngoài nodes[]
                {
                    if (Check_Ishape_Y(nMinY_X, nMaxY_X, y))
                    {
                        // Console.WriteLine($"TH X {type}: ({pMinY.X}, {pMinY.Y}) -> ({pMinY.X}, {y}) -> ({pMaxY.X}, {y}) -> ({pMaxY.X}, {pMaxY.Y})");
                        return y;
                    }
                    y += type;
                }
            }
            return -1;
        }

        // Kiểm tra thêm các đường dọc
        private int Check_Ushape_Y(Node n1, Node n2, int type)
        {
            int nMinX_X = n1.X, nMaxX_X = n2.X;
            int nMinX_Y = n1.Y, nMaxX_Y = n2.Y;

            if (n1.X > n2.X)
            {
                nMinX_X = n2.X; nMaxX_X = n1.X;
                nMinX_Y = n2.Y; nMaxX_Y = n1.Y;
            }

            int x = (type == -1) ? (nMinX_X - 1) : (nMaxX_X + 1);
            int col = (type == -1) ? nMaxX_Y : nMinX_Y;

            if (Check_Ishape_Y(nMinX_X, nMaxX_X, col))
            {
                while (x >= 0 && x <= (rows + 1))
                {
                    if (Check_Ishape_X(nMinX_Y, nMaxX_Y, x))
                    {
                        // Console.WriteLine($"TH Y {type}: ({pMinX.X}, {pMinX.Y}) -> ({x}, {pMinX.Y}) -> ({x}, {pMaxX.Y}) -> ({pMaxX.X}, {pMaxX.Y})");
                        return x;
                    }
                    x += type;
                }
            }
            return -1;
        }

        /*
        // Bảo, hàm tạo cutPath từ danh sách đường đi đầy đủ
        public List<Node> ExtractCutPath(List<Node> fullPath)
        {
            List<Node> cutPath = new List<Node>();
            if (fullPath == null || fullPath.Count == 0)
                return cutPath;

            // Thêm node đầu tiên
            cutPath.Add(fullPath[0]);

            // Tìm các điểm rẽ nhánh
            for (int i = 1; i < fullPath.Count - 1; i++)
            {
                Direction dir1 = GetDirection(fullPath[i - 1], fullPath[i]);
                Direction dir2 = GetDirection(fullPath[i], fullPath[i + 1]);

                if (dir1 != dir2) // Phát hiện rẽ nhánh
                {
                    cutPath.Add(fullPath[i]);
                    if (cutPath.Count == 3) // Đảm bảo chỉ lấy tối đa 2 điểm rẽ nhánh
                        break;
                }
            }

            // Thêm node cuối cùng
            cutPath.Add(fullPath[fullPath.Count - 1]);

            return cutPath;
        }
        */

        //Bảo, hàm này để nhận biết giữa 2 node được chọn có đường đi hay không
        public bool HasPath(Node startNode, Node endNode)
        {
            // Gọi hàm FindPath để tìm đường đi
            Console.WriteLine("HasPath goi FindPath");
            var path = FindPath(startNode, endNode);
            // var path = FindPathDijkstra(startNode, endNode);

            // Kiểm tra nếu có đường đi (nếu path không null và có ít nhất 1 node)
            return (path != null && path.Count > 0);
        }

        public bool HasPath(PictureBox picBox1, PictureBox picBox2)
        {
            Node node1 = GetNodeFromPictureBox(picBox1);
            Node node2 = GetNodeFromPictureBox(picBox2);

            return HasPath(node1, node2);
        }

        // Hàm DrawPath này vẽ trên cutPath
        public async Task DrawPath(List<Node> path)
        {
            if (path == null || path.Count < 2)
                return; // Không có đường đi hoặc đường đi không hợp lệ

            // Duyệt qua các đoạn trong fullPath và vẽ từng đoạn
            for (int i = 0; i < path.Count - 1; i++)
            {
                Node startNode = path[i];
                Node endNode = path[i + 1];

                // Lấy tọa độ trung tâm của hai PictureBox tương ứng với startNode và endNode
                Point start = GetCenterOfPictureBox(startNode.pictureBox);
                Point end = GetCenterOfPictureBox(endNode.pictureBox);

                if (startNode.pictureBox != null && endNode.pictureBox != null)
                {
                    using (Graphics g = startNode.pictureBox.Parent.CreateGraphics())
                    {
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Vẽ mượt hơn
                        using (Pen pen = new Pen(Color.Red, 3))
                        {
                            g.DrawLine(pen, start, end); // Vẽ đoạn đường nối hai điểm
                        }
                    }
                }
            }

            // Tạm dừng để người dùng có thể thấy đường đi
            await Task.Delay(500);

            // Sau khi hiển thị, xóa đường đi
            ClearPath(path);
        }

        private Point GetCenterOfPictureBox(PictureBox pictureBox)
        {
            if (pictureBox == null)
                return Point.Empty;

            int centerX = pictureBox.Location.X + pictureBox.Width / 2;
            int centerY = pictureBox.Location.Y + pictureBox.Height / 2;
            return new Point(centerX, centerY);
        }
        private void ClearPath(List<Node> path)
        {
            if (path == null || path.Count == 0)
                return;

            // Xóa đường đi bằng cách làm mới `Parent` của `PictureBox`
            Control parent = path[0].pictureBox?.Parent;
            if (parent != null)
            {
                parent.Refresh(); // Làm mới để xóa các đường vẽ
            }
        }

        // Bảo, hàm này để gọi việc kiểm tra và xáo lại
        public void HandleRefresh(Dictionary<PictureBox, Image> originalImages)
        {
            
            int cnt = 0;
            foreach (var node in nodes)
            {
                if (node.isTraversable == false)
                    break;
                cnt++;
            }
            if (cnt == (rows + 2) * (cols + 2)) // Để ko phải kiểm tra khi đã hoàn thành trò chơi
                return;
            
            bool flag = HasValidPairs();
            while (flag == false)
            {
                RepositionImages(originalImages); // Xáo trộn lại hình ảnh
                if (HasValidPairs())
                    flag = true;
            }

            // Debug isTraversable
            foreach (var node in nodes)
            {
                Console.Write($"Node [{node.X}, {node.Y}]: ");
                Console.WriteLine(node.isTraversable);
            }    
        }

        public bool HasValidPairs() // Bảo, hàm này để kiểm tra còn cặp nào hợp lệ không
        {
            // Lấy danh sách tất cả các PictureBox còn hiển thị
            List<PictureBox> visibleBoxes = new List<PictureBox>();
            foreach (var pictureBox in pictureGrid)
            {
                if (pictureBox.Visible)
                    visibleBoxes.Add(pictureBox);
            }

            // Duyệt qua tất cả các cặp và kiểm tra
            for (int i = 0; i < visibleBoxes.Count; i++)
            {
                for (int j = i + 1; j < visibleBoxes.Count; j++)
                {
                    if (visibleBoxes[i].Image != visibleBoxes[j].Image)
                        continue;
                    
                    Node node1 = GetNodeFromPictureBox(visibleBoxes[i]);
                    Node node2 = GetNodeFromPictureBox(visibleBoxes[j]);

                    // Sử dụng HasPath để kiểm tra đường đi
                    Console.WriteLine("HasValidPair goi HasPath");
                    if (node1 != null && node2 != null && HasPath(node1, node2))
                    {
                        // Thao tác này đảm bảo trả hiện trạng node về như cũ sau khi gọi FindPath
                        node1.isTraversable = false;
                        node2.isTraversable = false;

                        return true; // Tìm thấy cặp hợp lệ
                    }    
                }
            }

            return false; // Không tìm thấy cặp hợp lệ
        }

        // Hàm này xáo lại vị trí các hình ảnh (cấu trúc của nodes và pictureGrid ko thay đổi)
        public void RepositionImages(Dictionary<PictureBox, Image> originalImages)
        {
            // Xóa toàn bộ trong Dictionary của originalImages
            originalImages.Clear();
            
            // Lấy danh sách các hình ảnh (được gắn vào các picBox) còn lại
            List<Image> availableImages = new List<Image>();

            for (int i = 1; i < (rows + 1); i++) // Bỏ viền ngoài
            {
                for (int j = 1; j < (cols + 1); j++) // Bỏ viền ngoài
                {
                    if (pictureGrid[i, j].Visible && pictureGrid[i, j].Image != null) // Chỉ xét picBox còn hiển thị và có hình ảnh
                        availableImages.Add(pictureGrid[i, j].Image);

                    pictureGrid[i, j].Image = null;
                    pictureGrid[i, j].Visible = false;
                }
            }
            Console.WriteLine($"So img co trong availibleImages la: [{availableImages.Count}] / {rows * cols}");

            Random r = new Random();
            // Gắn lại các hình ảnh vào các picBox
            for (int i = 0; i < availableImages.Count; i++)
            {
                Tuple<int, int> pos;
                while (true)
                {
                    int temp = r.Next(0, (rows * cols));
                    pos = CalculatePosition(temp);

                    if (pictureGrid[pos.Item1, pos.Item2].Image == null) // Kiểm tra ô được quay trúng
                        break;                                           // có được gắn hình chưa
                }

                pictureGrid[pos.Item1, pos.Item2].Image = availableImages[i];
                pictureGrid[pos.Item1, pos.Item2].Visible = true;
            }

            foreach (PictureBox pictureBox in pictureGrid)
            {
                if (!originalImages.ContainsKey(pictureBox))
                {
                    originalImages[pictureBox] = pictureBox.Image;
                }
            }
            /*
            // Bảo, check thử originalImages
            foreach (var image in originalImages)
            {
                Console.WriteLine(image.Value);
            }
            */
            for (int i = 1; i < (rows + 1); i++) // Tái thiết lại thuộc tính isTraversable của từng node
            {
                for (int j = 1;  j < (cols + 1); j++)
                {
                    if (nodes[i, j].pictureBox.Visible)
                        nodes[i, j].isTraversable = false;
                    else
                        nodes[i, j].isTraversable = true;
                }
            }
        }
        
        // Bảo, hàm ánh xạ từ số ngẫu nhiên sang tọa độ (không tính viền)
        Tuple<int, int> CalculatePosition(int num)
        {
            int rowPos = (num / cols) + 1; // +1 để không tính biên
            int colPos = (num % cols) + 1;

            return new Tuple<int, int>(rowPos, colPos);
        }
    }

}
