using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace PikachuClassic
{
    public partial class MainGame : Form //GameController
    {
        // Thuộc tính của màn chơi
        private Grid grid;
        private int cols = 2;
        private int rows = 2;

        //Logic matching
        private bool firstGuess, secondGuess;
        private PictureBox firstGuessBox, secondGuessBox;
        private PictureBox[,] pictureGrid;
        private Dictionary<PictureBox, Image> originalImages = new Dictionary<PictureBox, Image>();

        // Điểm số + Thời gian chơi
        private int score = 0;
        private Timer time;

        public MainGame()
        {
            // Khởi tạo puzzle
            InitializeComponent();
            grid = new Grid(gridPanel, cols, rows);
            grid.GenerateGrid();
            AddEventToPictureBoxes();

            // Khởi tạo logic
            firstGuess = secondGuess = false;
        }
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

            if (clickedBox == null || clickedBox.Image == null || firstGuessBox == clickedBox) return;

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
        private void label1_Click(object sender, EventArgs e)
        {

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

                // Thêm điểm
               // AddScore(10);

                // Kiểm tra xem game đã kết thúc chưa
               CheckIfTheGameIsFinished();
            }
            else
            {
                // Nếu không khớp, đặt lại màu cho các ô
                firstGuessBox.Image = originalImages[firstGuessBox];
                secondGuessBox.Image = originalImages[secondGuessBox];
            }

            // Đặt lại trạng thái
            firstGuess = secondGuess = false;
            firstGuessBox = secondGuessBox = null;
        }
        private void CheckIfTheGameIsFinished()
        {
            if (grid.AllPictureBoxesHidden())
            {
                MessageBox.Show("Chúc mừng! Bạn đã hoàn thành trò chơi!");
            }
        }
    }
}