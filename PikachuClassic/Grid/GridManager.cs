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
        private int rows = 4;

        //Logic matching
        private bool firstGuess, secondGuess;
        private PictureBox firstGuessBox, secondGuessBox;
        private PictureBox[,] pictureGrid;
        private Dictionary<PictureBox, Image> originalImages = new Dictionary<PictureBox, Image>();

        public void GenerateGrid(Panel panel)
        {
            grid = new Grid(panel, rows, cols); 
            grid.GenerateGrid();

            AddEventToPictureBoxes(); 

            foreach (PictureBox pictureBox in pictureGrid)
            {
                if (!originalImages.ContainsKey(pictureBox))
                {
                    originalImages[pictureBox] = pictureBox.Image;
                }
            }

            // Khởi tạo logic
            firstGuess = secondGuess = false;
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

            if (clickedBox == null || !clickedBox.Visible || clickedBox.Image == null) return;

            // Kiểm tra nếu người chơi bấm vào ô đầu tiên đã chọn để huỷ chọn
            if (firstGuess && firstGuessBox == clickedBox)
            {
                // Khôi phục ảnh gốc của ô và huỷ chọn ô đầu tiên
                firstGuessBox.Image = originalImages[firstGuessBox];
                firstGuess = false;
                firstGuessBox = null;
                return; // Kết thúc mà không tiếp tục logic đoán
            }

            if (!firstGuess)
            {
                firstGuess = true;
                firstGuessBox = clickedBox; // Gán ô đầu tiên được chọn

                // Lưu lại hình ảnh gốc nếu chưa có trong danh sách
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

                // Lưu lại hình ảnh gốc nếu chưa có trong danh sách
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
                ScoreGroup score= grid.GetScoreForImage(originalImages[firstGuessBox]);
                // Thêm điểm
                GameManager.Instance.AddScore((int)score, GameManager.Instance.GetCurrentPlayer());


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
            GameManager.Instance.SwitchTurn();

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
        public PictureBox[,] GetPictureBoxes()
        {
            return pictureGrid;
        }
        public void ResetData()
        {
            grid = null;
            firstGuess = secondGuess = false;
            firstGuessBox = secondGuessBox = null;
            originalImages.Clear();

        }
    }
}
