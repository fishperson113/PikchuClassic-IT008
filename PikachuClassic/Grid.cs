using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    public class Grid
    {
        // Thuộc tính của grid
        private int rows;
        private int cols;
        private PictureBox[,] pictureGrid;
        private Panel gridPanel;
        private int cellSize = 50;

        public Grid(Panel panel, int rows, int cols)
        {
            this.gridPanel = panel;
            this.rows = rows;
            this.cols = cols;
            this.pictureGrid = new PictureBox[rows, cols];

            CalculateCellSize();
            gridPanel.BackgroundImageLayout = ImageLayout.Stretch;
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
        }
        private void AssignImagesToGrid()
        {
            int totalCells = rows * cols;
            if (totalCells % 2 != 0)
            {
                MessageBox.Show("Số lượng ô phải là số chẵn để có thể gán đủ các cặp hình!");
                return;
            }

            List<Image> imagesList = new List<Image>();
            for (int i = 0; i < totalCells / 2; i++) // Chia đôi vì mỗi ảnh xuất hiện 2 lần
            {
                Image img = (Image)Properties.Resources.ResourceManager.GetObject($"_{i}");
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
    }
}
