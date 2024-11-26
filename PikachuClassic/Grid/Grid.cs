using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
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
            this.pictureGrid = new PictureBox[rows, cols];

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
            cellSize = Math.Min(availableWidth / cols, availableHeight / rows);
        }
        public void GenerateGrid()
        {
            int totalGridWidth = cols * cellSize;
            int totalGridHeight = rows * cellSize;

            int offsetX = (gridPanel.Width - totalGridWidth) / 2; // Căn giữa theo chiều ngang
            int offsetY = (gridPanel.Height - totalGridHeight) / 2; // Căn giữa theo chiều dọc

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
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
            initializeNodes(nodes);
            setNeighbors(nodes);
            // Hết Bảo
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
            int totalCells = rows * cols;
            if (totalCells % 2 != 0)
            {
                MessageBox.Show("Số lượng ô phải là số chẵn để có thể gán đủ các cặp hình!");
                return;
            }
            Shuffle(allImages);

            for (int i = 0; i < totalCells / 2; i++) // Chia đôi vì mỗi ảnh xuất hiện 2 lần
            {
                Image img = allImages[i % allImages.Count]; // i% allImages.Count để đảm bảo totalCells/2 < allImages.Count (tránh lỗi index out of range) nếu vượt quá thì nó quay về index=0
                imagesList.Add(img); // Thêm lần 1
                imagesList.Add(img); // Thêm lần 2 (tạo cặp)
            }

            Shuffle(imagesList);

            // Gán hình ảnh từ danh sách đã xáo trộn vào các PictureBox trong grid
            int imageIndex = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
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
        public void initializeNodes(Node[,] node)
        {
            // Khởi tạo node
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    PictureBox picBox = (i > 0 && i <= rows && j > 0 && j <= cols) ? pictureGrid[i - 1, j - 1] : null;
                    //dòng này hiểu là 1 node bên pictureGrid có tọa độ là 0,0 thì bên nodes có tọa độ là 1,1
                    //hàm GenerateGrid bên grid.cs khởi tạo sẵn giá trị rows vs cols cho cái pictureGrid riêng rồi
                    //nên Bảo gắn cái pictureGrid đó vô cái nền đồ thị thôi

                    if (picBox != null)
                        // Thiết lập node có hình
                        nodes[i, j] = new Node(picBox, false, i, j);
                    else
                        // Thiết lập node viền ngoài
                        nodes[i, j] = new Node(null, true, i, j);
                }
            }
        }

        //Bảo, khởi tạo danh sách Neighbors cho node
        public void setNeighbors(Node[,] nodes)
        {
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    if (i > 0 && nodes[i - 1, j] != null)
                        nodes[i, j].AddNeighbor(nodes[i - 1, j]); // Trên
                    if (i < nodes.GetLength(0) - 1 && nodes[i + 1, j] != null)
                        nodes[i, j].AddNeighbor(nodes[i + 1, j]); // Dưới
                    if (j > 0 && nodes[i, j - 1] != null)
                        nodes[i, j].AddNeighbor(nodes[i, j - 1]); // Trái
                    if (j < nodes.GetLength(1) - 1 && nodes[i, j + 1] != null)
                        nodes[i, j].AddNeighbor(nodes[i, j + 1]); // Phải
                }
            }
        }

        public PictureBox[,] GetPictureBoxes()
        {
            return pictureGrid;
        }
        
        public bool AllPictureBoxesHidden()
        {
            // Duyệt qua tất cả các PictureBox trong grid
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
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
        public ScoreGroup GetScoreForImage(Image image)
        {
            if (imageScoreGroups.TryGetValue(image, out ScoreGroup score))
            {
                Console.WriteLine($"Điểm của ảnh: {score}");
                return score;
            }
            Console.WriteLine("Không tìm thấy ảnh trong dictionary");
            return ScoreGroup.Group10; // Điểm mặc định nếu không tìm thấy ảnh
        }

        // Bảo, dùng để loại node đã bị vô hiệu hóa khi chọn 2 hình giống nhau + cập nhật lại danh sách kề
        internal void RemoveNodes(PictureBox firstBox, PictureBox secondBox)
        {
            Node firstNode = GetNodeFromPictureBox(firstBox);
            Node secondNode = GetNodeFromPictureBox(secondBox);

            firstNode.isTraversable = true;
            secondNode.isTraversable = true;

            //thử nghiệm cho vẽ đường, xíu sẽ cmt lại
            firstNode.PictureBox = null;
            secondNode.PictureBox = null;
        }

        //Bảo, lấy ánh xạ từ vị trí của PictureBox -> vị trí của node
        public Node GetNodeFromPictureBox(PictureBox pictureBox)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (pictureGrid[i, j] == pictureBox)
                    {
                        return nodes[i + 1, j + 1];
                    }
                }
            }
            return null; // Trả về null nếu không tìm thấy
        }

        //Bảo, hàm này để tìm đường đi và trả về danh sách các node đã đi qua
        public List<Node> findPath(Node startNode, Node endNode)
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
                    if (directionFrom.ContainsKey(current) && directionFrom[current] != direction) // vừa rẽ hướng
                    {
                        if (pathChangesExceeded(cameFrom, neighbor, 2)) //Nguyên bản: cameFrom, current, 2
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
                //nghi ngờ, nguyên bản: (temp, cameFrom[temp]). định sửa: (cameFrom[temp], temp)
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


        //Bảo, hàm này để nhận biết giữa 2 node được chọn có đường đi hay không
        public bool HasPath(Node startNode, Node endNode)
        {
            // Gọi hàm FindPath để tìm đường đi
            var path = findPath(startNode, endNode);

            // Kiểm tra nếu có đường đi (nếu path không null và có ít nhất 1 node)
            return (path != null && path.Count > 0);
        }

        public bool HasPath(PictureBox picBox1, PictureBox picBox2)
        {
            Node node1 = GetNodeFromPictureBox(picBox1);
            Node node2 = GetNodeFromPictureBox(picBox2);

            return HasPath(node1, node2);
        }

        public bool HasPath(List<Node> path) //ý tưởng là giảm số lần chạy findpath lại
        {
            return (path != null && path.Count > 0);
        }

        public async Task DrawPath(List<Node> cutPath, Panel gridPanel, int delayMs = 500)
        {
            if (cutPath == null || cutPath.Count < 2)
                return;

            // Kiểm tra Panel để lấy Graphics
            using (Graphics graphics = gridPanel.CreateGraphics())
            {
                Pen pen = new Pen(Color.Blue, 5); // Định nghĩa bút vẽ màu xanh và độ dày 5

                // Vẽ từng đoạn của đường đi
                for (int i = 0; i < cutPath.Count - 1; i++)
                {
                    // Lấy tọa độ từ node hiện tại và node kế tiếp //***
                    Point startPoint = new Point(cutPath[i].Y * cellSize, cutPath[i].X * cellSize);
                    Point endPoint = new Point(cutPath[i + 1].Y * cellSize, cutPath[i + 1].X * cellSize);

                    // Debug: In tọa độ ra console để kiểm tra
                    Console.WriteLine($"Vẽ từ ({startPoint.X}, {startPoint.Y}) đến ({endPoint.X}, {endPoint.Y})");

                    // Vẽ đường thẳng
                    graphics.DrawLine(pen, startPoint, endPoint);
                }
            }

            // Làm mới giao diện để xóa đường sau khi vẽ hoàn thành
            await Task.Delay(1000); // Đợi 1 giây để người dùng thấy rõ đường đi
            gridPanel.Invalidate();
        }

        /*
        // Bảo, lấy tọa độ trung tâm của PictureBox (cái này thú vị)
        private Point GetPictureBoxCenter(PictureBox pictureBox)
        {
            if (pictureBox == null) return Point.Empty;

            return new Point(
                pictureBox.Location.X + pictureBox.Width / 2,
                pictureBox.Location.Y + pictureBox.Height / 2
            );
        }
        */

    }
    
}
