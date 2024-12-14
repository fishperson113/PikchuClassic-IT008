// PikachuClassic/UI/Forms/GameForm.cs:path/to/file
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Windows.Forms;
using PikachuClassic.Core.Game;
using PikachuClassic.Core.Models;
using PikachuClassic.Infrastructure.Network.Enums;

namespace PikachuClassic.UI.Forms
{
    public partial class GameForm : Form
    {
        private Tuple<int, int> firstSelected = null;
        private int[,] currentGrid; // Lưu grid hiện tại
        private string playerId;
        public GameForm(GameMode gameMode)
        {
            InitializeComponent();
            // Đăng ký các sự kiện từ GameManager
            GameManager.Instance.OnGridUpdated += RenderGrid;
            GameManager.Instance.OnScoreUpdated += UpdateScores;
            GameManager.Instance.OnTimeUpdated += UpdateTimer;
            GameManager.Instance.OnGameFinished += HandleGameFinished;

            // Khởi động trò chơi
            Task.Run(async () =>
            {
                try
                {
                    // Nếu là PvP, lấy Player ID trước
                    if (gameMode == GameMode.PvP)
                    {
                        if (!await GetPlayerIdFromUser())
                        {
                            this.Invoke(new Action(() => this.Close()));
                            return;
                        }
                    }

                    // Khởi tạo game
                    await GameManager.Instance.StartGame(gameMode);

                    // Nếu là PvP và có Player ID, join game
                    if (gameMode == GameMode.PvP && !string.IsNullOrEmpty(playerId))
                    {
                        var isJoined = await GameManager.Instance.JoinGame(playerId);
                        if (!isJoined)
                        {
                            MessageBox.Show("Failed to join game", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.Invoke(new Action(() => this.Close()));
                            return;
                        }
                        Console.WriteLine($"Successfully joined game with ID: {playerId}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to start game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Invoke(new Action(() => this.Close()));
                }
            });
        }
        private async Task<bool> GetPlayerIdFromUser()
        {
            bool success = false;

            // Sử dụng Invoke với Action cụ thể
            this.Invoke((MethodInvoker)delegate
            {
                using (var inputForm = new Form())
                {
                    inputForm.Width = 300;
                    inputForm.Height = 150;
                    inputForm.Text = "Player ID Required";
                    inputForm.StartPosition = FormStartPosition.CenterParent;

                    TextBox textBox = new TextBox() { Left = 50, Top = 20, Width = 200 };
                    Button confirmButton = new Button() { Text = "OK", Left = 50, Top = 50, DialogResult = DialogResult.OK };
                    Button cancelButton = new Button() { Text = "Cancel", Left = 150, Top = 50, DialogResult = DialogResult.Cancel };

                    inputForm.Controls.AddRange(new Control[] { textBox, confirmButton, cancelButton });

                    var result = inputForm.ShowDialog();
                    if (result == DialogResult.Cancel)
                    {
                        success = false;
                        return;
                    }

                    string input = textBox.Text;
                    if (input.Trim().Length > 0)
                    {
                        playerId = input.Trim();
                        success = true;
                    }
                    else
                    {
                        MessageBox.Show("Player ID cannot be empty!", "Invalid Input",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        success = false;
                    }
                }
            });

            return success;
        }
        private void RenderGrid(int[,] grid)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => RenderGrid(grid)));
                return;
            }

            currentGrid = grid; // Lưu lại grid hiện tại

            foreach (Control control in gridPanel.Controls)
            {
                if (control is PictureBox pictureBox)
                {
                    var position = (Tuple<int, int>)pictureBox.Tag;
                    if (grid[position.Item1, position.Item2] == 0)
                    {
                        // Animate fade out
                        FadeOutPictureBox(pictureBox);
                    }
                }
            }

            if (gridPanel.Controls.Count == 0)
            {
                RenderFullGrid(grid);
            }
        }
        private void FadeOutPictureBox(PictureBox pictureBox)
        {
            // Animation khi xóa PictureBox
            var fadeTimer = new Timer { Interval = 50 };
            double opacity = 1.0;

            fadeTimer.Tick += (s, e) =>
            {
                opacity -= 0.1;
                if (opacity <= 0)
                {
                    fadeTimer.Stop();
                    pictureBox.Visible = false;
                    pictureBox.Enabled = false;
                    gridPanel.Controls.Remove(pictureBox);
                    pictureBox.Dispose();
                }
                else
                {
                    // Giảm độ trong suốt
                    pictureBox.Image = SetImageOpacity(pictureBox.Image, opacity);
                }
            };

            fadeTimer.Start();
        }
        private void RenderFullGrid(int[,] grid)
        {
            gridPanel.Controls.Clear();
            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);
            int cellWidth = gridPanel.Width / cols;
            int cellHeight = gridPanel.Height / rows;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    int imageId = grid[i, j];
                    if (imageId != 0) // Chỉ render các ô không rỗng
                    {
                        PictureBox pictureBox = new PictureBox
                        {
                            Width = cellWidth,
                            Height = cellHeight,
                            BorderStyle = BorderStyle.FixedSingle,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Location = new Point(j * cellWidth, i * cellHeight),
                            Tag = new Tuple<int, int>(i, j),
                            Image = GetImageById(imageId)
                        };

                        pictureBox.Click += PictureBox_Click;
                        gridPanel.Controls.Add(pictureBox);
                    }
                }
            }
        }
        private Image SetImageOpacity(Image image, double opacity)
        {
            if (image == null) return null;

            Bitmap bmp = new Bitmap(image.Width, image.Height);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = (float)opacity;
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height),
                    0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }
        private async void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickedBox = sender as PictureBox;
            if (clickedBox == null || currentGrid == null)
                return;

            var position = (Tuple<int, int>)clickedBox.Tag;
            int row = position.Item1;
            int col = position.Item2;

            if (firstSelected == null)
            {
                firstSelected = new Tuple<int, int>(row, col);
                // Highlight selected box
                clickedBox.BorderStyle = BorderStyle.Fixed3D;
                return;
            }

            try
            {
                // Disable clicks while processing
                gridPanel.Enabled = false;

                // Gửi nước đi tới server
                await GameManager.Instance.MakeMove(
                    row1: firstSelected.Item1,
                    col1: firstSelected.Item2,
                    row2: row,
                    col2: col
                );

                // Reset selection
                firstSelected = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error making move: {ex.Message}");
            }
            finally
            {
                // Re-enable clicks
                gridPanel.Enabled = true;
                clickedBox.BorderStyle = BorderStyle.None;
            }
        }

        private void UpdateScores(int score, Player player)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateScores(score, player)));
                return;
            }

            if (player.Id == 1)
                scoreLbP1.Text = $"Score: {score}";
            else
                scoreLbP2.Text = $"Score: {score}";
        }

        private void UpdateTimer(int remainingTime)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateTimer(remainingTime)));
                return;
            }

            timeLb.Text = $"Time: {remainingTime}s";
        }

        private void HandleGameFinished()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(HandleGameFinished));
                return;
            }

            // Chuyển đến màn hình kết thúc game
            FormManager.Instance.NavigateToWinScreen();
            this.Close();
        }

        private void GameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Hủy đăng ký sự kiện khi form đóng
            GameManager.Instance.OnGridUpdated -= RenderGrid;
            GameManager.Instance.OnScoreUpdated -= UpdateScores;
            GameManager.Instance.OnTimeUpdated -= UpdateTimer;
            GameManager.Instance.OnGameFinished -= HandleGameFinished;
        }

        private Image GetImageById(int imageId)
        {
            return Properties.Resources.ResourceManager.GetObject($"_{imageId}") as Image;
        }
    }
}