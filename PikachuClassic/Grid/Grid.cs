using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        // Cache cho 24 hoán vị
        private static readonly List<int[]> neighborPermutations;

        static Grid()
        {
            // Khởi tạo 24 hoán vị của [0,1,2,3]
            neighborPermutations = GeneratePermutations(new int[] { 0, 1, 2, 3 }).ToList();
        }
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

            // Bảo, tạo ra tempImageList để fix cứng cặp đầu
            List<Image> tempImagesList = new List<Image>();
            for (int i = 1; i < (totalCells / 2); i++) // i = 1 để ko xáo cặp đầu
            {
                Image img = allImages[i % allImages.Count]; // Dương, i% allImages.Count để đảm bảo totalCells/2 < allImages.Count (tránh lỗi index out of range) nếu vượt quá thì nó quay về index=0
                tempImagesList.Add(img);
                tempImagesList.Add(img);
            }
            Shuffle(tempImagesList);

            imagesList.Add(allImages[0]);
            imagesList.Add(allImages[0]);
            for (int i = 0; i < (totalCells - 2); i++)
            {
                imagesList.Add(tempImagesList[i]);
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

        private List<Node> FindPath(Node startNode, Node endNode, int[] permutation = null)
        {
            if (startNode == null || endNode == null)
                return null;

            if (startNode.isNeighbor(endNode))
                return new List<Node> { startNode, endNode };

            // Áp dụng permutation nếu được chỉ định
            if (permutation != null)
            {
                foreach (var node in GetAllNodes())
                {
                    node.ApplyPermutation(permutation);
                }
            }

            Queue<Node> queue = new Queue<Node>();
            Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
            HashSet<Node> visited = new HashSet<Node>();

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                Node current = queue.Dequeue();

                if (current == endNode)
                    break;

                foreach (var neighbor in current.Neighbors)
                {
                    if (visited.Contains(neighbor) || (!neighbor.isTraversable && neighbor != endNode))
                        continue;

                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }
            if (permutation != null)
            {
                foreach (var node in GetAllNodes())
                {
                    node.ResetNeighbors();
                }
            }

            if (!cameFrom.ContainsKey(endNode))
                return null;

            List<Node> path = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != null)
            {
                path.Add(currentNode);
                cameFrom.TryGetValue(currentNode, out currentNode);
            }
            path.Reverse();

            return path;
        }
        private bool IsValidPath(List<Node> path)
        {
            if (path == null || path.Count < 2) return false;

            int bends = 0;
            Direction? lastDirection = null;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Direction currentDirection = GetDirection(path[i], path[i + 1]);

                if (lastDirection.HasValue && lastDirection.Value != currentDirection)
                {
                    bends++;
                    if (bends > 2) return false;
                }

                lastDirection = currentDirection;
            }

            Console.WriteLine($"Path validation: Bends={bends}, Length={path.Count}");
            return true;
        }
        private Direction GetDirection(Node from, Node to)
        {
            if (to.X > from.X) return Direction.Down;
            if (to.X < from.X) return Direction.Up;
            if (to.Y > from.Y) return Direction.Right;
            return Direction.Left;
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

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

        public bool HasPath(Node startNode, Node endNode)
        {
            var allPaths = GetAllPossiblePaths(startNode, endNode);
            return ValidatePaths(allPaths);
        }

        public bool HasPath(PictureBox picBox1, PictureBox picBox2)
        {
            Node node1 = GetNodeFromPictureBox(picBox1);
            Node node2 = GetNodeFromPictureBox(picBox2);

            return HasPath(node1, node2);
        }

        private Point GetCenterOfPictureBox(PictureBox pictureBox)
        {
            if (pictureBox == null)
                return Point.Empty;

            int centerX = pictureBox.Location.X + pictureBox.Width / 2;
            int centerY = pictureBox.Location.Y + pictureBox.Height / 2;
            return new Point(centerX, centerY);
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
                for (int j = 1; j < (cols + 1); j++)
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
        private static IEnumerable<int[]> GeneratePermutations(int[] array)
        {
            if (array.Length == 1)
                yield return array;
            else
            {
                for (int i = 0; i < array.Length; i++)
                {
                    var remaining = new int[array.Length - 1];
                    Array.Copy(array, 0, remaining, 0, i);
                    Array.Copy(array, i + 1, remaining, i, array.Length - i - 1);
                    foreach (var permutation in GeneratePermutations(remaining))
                    {
                        var result = new int[array.Length];
                        result[0] = array[i];
                        Array.Copy(permutation, 0, result, 1, permutation.Length);
                        yield return result;
                    }
                }
            }
        }
        private List<List<Node>> GetAllPossiblePaths(Node startNode, Node endNode)
        {
            List<List<Node>> allPaths = new List<List<Node>>();

            foreach (var permutation in neighborPermutations)
            {
                var path = FindPath(startNode, endNode, permutation);
                if (path != null)
                {
                    allPaths.Add(path);
                }
            }

            return allPaths;
        }

        // Validate paths và trả về true ngay khi tìm thấy đường hợp lệ
        private bool ValidatePaths(List<List<Node>> paths)
        {
            foreach (var path in paths)
            {
                if (IsValidPath(path))
                    return true;
            }
            return false;
        }
        public List<Node> FindPath(Node startNode, Node endNode)
        {
            var allPaths = GetAllPossiblePaths(startNode, endNode);
            foreach (var path in allPaths)
            {
                if (IsValidPath(path))
                    return path;
            }
            return null;
        }
        public int GetRows()
        {
            return rows+2;
        }
        public int GetCols()
        {
            return cols+2;
        }
    }
}
