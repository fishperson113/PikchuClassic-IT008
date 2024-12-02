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
        private void UpdateScoreLabel(int score, Player player)
        {
            if (player == gameManager.Player1)
            {
                scoreLbP1.Text = $"Score P1: {gameManager.Player1.Score}";
            }
            else if (player == gameManager.Player2)
            {
                scoreLbP2.Text = $"Score P2: {gameManager.Player2.Score}";
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
        public GameController()
        {
            InitializeComponent();

            // Khởi tạo GameManager với gameMode đã chọn
            gameManager = GameManager.Instance;
            gridManager = GridManager.Instance;

            gridManager.GenerateGrid(gridPanel);

            gameManager.OnScoreUpdated += UpdateScoreLabel;
            gameManager.OnTimeUpdated += UpdateTimeLabel;
            gameManager.OnGameFinished += EndGame;

             // Bắt đầu đếm thời gian hoặc thiết lập trò chơi
        }
        ~GameController()
        {
            gameManager.OnScoreUpdated -= UpdateScoreLabel;
            gameManager.OnTimeUpdated -= UpdateTimeLabel;
            gameManager.OnGameFinished -= EndGame;
        }
        public void EndGame(bool isWin)
        {
            ResetData();
            Form endScreen = isWin ? (Form)new WinScreen() : new GameOverScreen();
            
            FormManager.Instance.OpenForm(endScreen);
        }

        private void ResetData()
        {
            gameManager.ResetData();
            gridManager.ResetData();
        }
        public void SetGameMode(string gameMode)
        {
            gameManager.Initialize(gameMode);
            gameManager.StartTimer(20);
        }
        private void GameController_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); 
        }

        //Click Tutorial icon
        private void button1_Click(object sender, EventArgs e)
        {
            FormManager.Instance.OpenForm(new GameTutorial());
        }

        private void timeLb_Click(object sender, EventArgs e)
        {

        }
    }
}