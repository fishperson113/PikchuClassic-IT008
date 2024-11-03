using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Security.Cryptography;
namespace PikachuClassic
{
    public class GridManager
    {
        #region Singleton
        private static GridManager instance;
        public static GridManager Instance
        {
            get
            {
                // Kiểm tra và khởi tạo instance nếu chưa tồn tại
                if (instance == null)
                {
                    instance = new GridManager();
                }
                return instance;
            }
        }
        public Grid Grid
        {
            get
            {
                return grid;
            }
        }
        #endregion
        // Thuộc tính của màn chơi
        private Grid grid;
        private int cols = 5;
        private int rows = 2;

        //Logic matching
        private bool firstGuess, secondGuess;
        private PictureBox firstGuessBox, secondGuessBox;
        private PictureBox[,] pictureGrid;
        private Dictionary<PictureBox, Image> originalImages = new Dictionary<PictureBox, Image>();

        private Node[,] nodes; // Bảo thêm, khai báo mảng 2 chiều các node đồ thị
        public void GenerateGrid(Panel panel)
        {
            //grid = new Grid(panel, cols, rows); // nguyên bản của anh Dương
            grid = new Grid(panel, rows, cols); // bản sửa của Bảo

            nodes = new Node[rows + 2, cols + 2]; // Bảo thêm

            grid.GenerateGrid();

            AddEventToPictureBoxes(); // đem dòng này lên

            //đem mớ này lên
            //Lưu hình ảnh gốc
            foreach (PictureBox pictureBox in pictureGrid)
            {
                if (!originalImages.ContainsKey(pictureBox))
                {
                    originalImages[pictureBox] = pictureBox.Image;
                }
            }

            // Bảo thêm
            // Khởi tạo node và thiết lập các node liền kề
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    PictureBox pictureBox = (i > 0 && i <= rows && j > 0 && j <= cols) ? pictureGrid[i - 1, j - 1] : null;
                    //dòng này hiểu là 1 node bên pictureGrid có tọa độ là 0,0 thì bên nodes có tọa độ là 1,1
                    //hàm GenerateGrid bên grid.cs khởi tạo sẵn giá trị rows vs cols cho cái pictureGrid riêng rồi
                    //nên Bảo gắn cái pictureGrid đó vô cái nền đồ thị thôi

                    if (pictureBox != null)
                    {
                        nodes[i, j] = new Node(pictureBox);
                    }
                    else
                    {
                        // Tạo node viền ngoài
                        nodes[i, j] = new Node(null, true); // true là để biết nó là node viền ngoài
                    }
                }
            }

            // Thiết lập các node liền kề
            for (int i = 0; i < nodes.GetLength(0); i++)
            {
                for (int j = 0; j < nodes.GetLength(1); j++)
                {
                    if (nodes[i, j] != null) // Kiểm tra node không phải null
                    {
                        // Chỉ thêm các node không phải viền ngoài vào danh sách kề
                        if (!nodes[i, j].IsBoundary)
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
            }
            // Hết Bảo

            //AddEventToPictureBoxes(); cmt dòng này lại r đem lên trên

            // Khởi tạo logic
            firstGuess = secondGuess = false;

            /* tạm đem cái này lên
            //Lưu hình ảnh gốc
            foreach (PictureBox pictureBox in pictureGrid)
            {
                if (!originalImages.ContainsKey(pictureBox))
                {
                    originalImages[pictureBox] = pictureBox.Image;
                }
            }
            */
        }
        #region Matching Logic and Tint Effect
        private void AddEventToPictureBoxes()
        {
            pictureGrid = grid.GetPictureBoxes();
            foreach (PictureBox pictureBox in pictureGrid)
            {
                pictureBox.Click += OnCellClick;
            }
        }
        private Image ApplyTintToImage(Image originalImage, Color tint, float opacity)
        {
            // Tạo một bitmap mới từ hình ảnh gốc
            Bitmap bmp = new Bitmap(originalImage.Width, originalImage.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Vẽ hình ảnh gốc lên bitmap
                g.DrawImage(originalImage, 0, 0);

                // Tạo lớp phủ màu với độ trong suốt
                using (Brush brush = new SolidBrush(Color.FromArgb((int)(opacity * 255), tint)))
                {
                    g.FillRectangle(brush, new Rectangle(0, 0, bmp.Width, bmp.Height));
                }
            }
            return bmp; // Trả về ảnh đã được phủ màu
        }
        private async void OnCellClick(object sender, EventArgs e)
        {
            PictureBox clickedBox = sender as PictureBox;

            if (clickedBox == null || !clickedBox.Visible || clickedBox.Image == null || firstGuessBox == clickedBox) return;

            if (!firstGuess)
            {
                firstGuess = true;
                firstGuessBox = clickedBox; // Gán ô đầu tiên được chọn

                // Lưu lại hình ảnh gốc
                if (!originalImages.ContainsKey(firstGuessBox))
                {
                    originalImages[firstGuessBox] = firstGuessBox.Image;
                }

                // Áp dụng hiệu ứng phủ bóng
                firstGuessBox.Image = ApplyTintToImage(originalImages[firstGuessBox], Color.Yellow, 0.3f); // Làm vàng ảnh

            }
            else if (!secondGuess)
            {
                secondGuess = true;
                secondGuessBox = clickedBox; // Gán ô thứ hai được chọn

                // Lưu lại hình ảnh gốc
                if (!originalImages.ContainsKey(secondGuessBox))
                {
                    originalImages[secondGuessBox] = secondGuessBox.Image;
                }

                // Áp dụng hiệu ứng chọn
                secondGuessBox.Image = ApplyTintToImage(originalImages[secondGuessBox], Color.Yellow, 0.3f); // Làm vàng ảnh

                await CheckIfThePuzzlesMatch();
            }
        }

        private async Task CheckIfThePuzzlesMatch()
        {
            await Task.Delay(500);

            // Kiểm tra xem hình ảnh của hai ô có khớp không bằng cách kiểm tra hình ảnh trc khi apply tint
            if (originalImages[firstGuessBox] == originalImages[secondGuessBox])
            {
                // Nếu khớp, ẩn các ô và vô hiệu hóa chúng
                firstGuessBox.Visible = false;
                secondGuessBox.Visible = false;

                // Bảo thêm
                // Loại bỏ node khỏi danh sách kề của các node khác và vô hiệu hóa nó
                RemoveNodes(firstGuessBox, secondGuessBox);

                // Thêm điểm
                GameManager.Instance.AddScore(10,GameManager.Instance.GetCurrentPlayer());


                // Kiểm tra xem game đã kết thúc chưa
                GameManager.Instance.CheckIfTheGameIsFinished();
            }
            else
            {
                // Nếu không khớp, đặt lại màu cho các ô
                firstGuessBox.Image = originalImages[firstGuessBox];
                secondGuessBox.Image = originalImages[secondGuessBox];
            }

            // Chuyển lượt sau mỗi lần đoán
            //GameManager.Instance.SwitchTurn(); cmt con bot lại
       
            // Đặt lại trạng thái của lượt đoán
            firstGuess = secondGuess = false;
            firstGuessBox = secondGuessBox = null;
        }
        #endregion
        public List<PictureBox> GetUnmatchedBoxes()
        {
            var unmatchedBoxes = new List<PictureBox>();
            foreach (var pictureBox in pictureGrid)
            {
                if (pictureBox.Visible) unmatchedBoxes.Add(pictureBox);
            }
            return unmatchedBoxes;
        }
        public bool AreImagesMatching(PictureBox first, PictureBox second)
        {
            if (originalImages.ContainsKey(first) && originalImages.ContainsKey(second))
            {
                return originalImages[first] == originalImages[second];
            }
            Debug.WriteLine("Hình ảnh không trong dictionary");
            return false;
        }
        public async Task BotClickCell(PictureBox clickedBox)
        {
            await Task.Delay(500);
            // Kiểm tra xem điều khiển (PictureBox) có đang trên luồng chính (UI) hay không
            // Mọi hành động liên quan đến UI trong Window form đều phải được thực hiện trên luồng chính
            if (clickedBox.InvokeRequired)
            {
                // Nếu không phải trên luồng UI, chuyển về luồng UI để gọi phương thức OnCellClick
                clickedBox.Invoke(new Action(() =>
                {
                    OnCellClick(clickedBox, EventArgs.Empty); // Gọi hàm OnCellClick trên luồng UI
                }));
            }
            else
            {
                // Nếu đã ở trên luồng UI, gọi trực tiếp OnCellClick
                OnCellClick(clickedBox, EventArgs.Empty);
            }

        }
        public PictureBox[,] GetPictureBoxes ()
        {
            return pictureGrid;
        }

        /*
        // Bảo
        // Tạm thời cmt lại cục FindPath
        
        // Các thuộc tính và hàm đã có...

        // Hàm tìm đường dựa trên BFS
        public List<Point> FindPath(int startX, int startY, int endX, int endY)
        {
            // Khởi tạo đồ thị với kích thước phù hợp
            int rows = pictureGrid.GetLength(0);
            int cols = pictureGrid.GetLength(1);
            int[,] grid = new int[rows + 2, cols + 2];

            // Xác định các ô có thể đi được trong đồ thị
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    grid[i + 1, j + 1] = pictureGrid[i, j].Visible ? 1 : 0;
                }
            }

            Point start = new Point(startX + 1, startY + 1);
            Point end = new Point(endX + 1, endY + 1);

            // Thuật toán BFS
            int[] dx = { -1, 0, 1, 0 }; // Mảng chứa các hướng đi
            int[] dy = { 0, 1, 0, -1 };
            Queue<Point> queue = new Queue<Point>();
            Dictionary<Point, Point> trace = new Dictionary<Point, Point>();

            queue.Enqueue(end);
            trace[end] = new Point(-2, -2);
            grid[start.X, start.Y] = 0;
            grid[end.X, end.Y] = 0;

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();

                if (current == start) break;

                for (int i = 0; i < 4; ++i)
                {
                    int x = current.X + dx[i];
                    int y = current.Y + dy[i];

                    while (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1) && grid[x, y] == 0)
                    {
                        Point next = new Point(x, y);
                        if (!trace.ContainsKey(next))
                        {
                            trace[next] = current;
                            queue.Enqueue(next);
                        }
                        x += dx[i];
                        y += dy[i];
                    }
                }
            }

            // Truy ngược lại đường đi
            List<Point> path = new List<Point>();
            if (trace.ContainsKey(start))
            {
                while (start.X != -2)
                {
                    path.Add(new Point(start.X - 1, start.Y - 1));
                    start = trace[start];
                }
                path.Reverse();
            }
            return path;
        }

        // Hết Bảo
        */

        // Bảo, class Node phục vụ cho việc đồ thị hóa grid và lập danh sách kề
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

        // Bảo, dùng để loại node đã bị vô hiệu hóa khi chọn 2 hình giống nhau + cập nhật lại danh sách kề
        private void RemoveNodes(PictureBox firstBox, PictureBox secondBox)
        {
            // Cập nhật danh sách kề của các node bị ảnh hưởng
            for (int i = 0; i < rows + 2; i++)
            {
                for (int j = 0; j < cols + 2; j++)
                {
                    Node node = nodes[i, j];
                    if (node != null && (node.PictureBox == firstBox || node.PictureBox == secondBox))
                    {
                        // Xóa node khỏi danh sách kề của các node liền kề
                        foreach (Node neighbor in node.Neighbors)
                        {
                            neighbor.Neighbors.Remove(node);
                        }
                        // Đặt node thành null để loại bỏ hoàn toàn
                        nodes[i, j] = null;
                    }
                }
            }
        }
    }

}
