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
    public partial class GameController : Form
    {
        private GridManager gridManager;
        private GameManager gameManager;
        public GameController()
        {
            InitializeComponent();
            gridManager = GridManager.Instance;
            gameManager = GameManager.Instance;
            
            gridManager.GenerateGrid(gridPanel);

            gameManager.OnScoreUpdated += UpdateScoreLabel;
            gameManager.OnTimeUpdated += UpdateTimeLabel;
            gameManager.StartTimer(20);
        }
        
        private void UpdateScoreLabel(int score, Player player)
        {
            if (player == gameManager.player1)
            {
                scoreLbP1.Text = $"Score P1: {gameManager.player1.Score}";
            }
            else if (player == gameManager.player2)
            {
                scoreLbP2.Text = $"Score P2: {gameManager.player2.Score}";
            }
        }
        private void UpdateTimeLabel(int timeRemaining)
        {
            timeLb.Text = $"Time Left: {timeRemaining}";
        }

        private void gridPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        //BACKUL
        public GameController(string gameMode)
        {
            InitializeComponent();

            // Khởi tạo GameManager với gameMode đã chọn
            gameManager = GameManager.Instance;
            gridManager = GridManager.Instance;

            gridManager.GenerateGrid(gridPanel);

            gameManager.Initialize(this, gameMode);
            gameManager.OnScoreUpdated += UpdateScoreLabel;
            gameManager.OnTimeUpdated += UpdateTimeLabel;
            gameManager.StartTimer(20); // Bắt đầu đếm thời gian hoặc thiết lập trò chơi
        }

        public void EndGame(bool isWin)
        {
            this.Hide();
            Form endScreen = isWin ? (Form)new WinScreen() : new GameOverScreen();
            endScreen.FormClosed += (s, args) => this.Close();
            endScreen.Show();
        }

        private void GameOverScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); //Thoát toàn bộ ứng dụng khi form cuối cùng bị đóng
        }


    }
}