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
        private string gameMode;

        public GameController()
        {
            InitializeComponent();
            RegisterEvents();
        }

        public void SetGameMode(string mode)
        {
            gameMode = mode;
            GameManager.Instance.Initialize(mode);
        }

        private void RegisterEvents()
        {
            GameManager.Instance.OnScoreUpdated += UpdateScore;
            GameManager.Instance.OnTimeUpdated += UpdateTime;
            GameManager.Instance.OnGameFinished += HandleGameFinished;
        }

        private void UpdateScore(int score, Player player)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateScore(score, player)));
                return;
            }
            
            if (player == GameManager.Instance.Player1)
                scoreLbP1.Text = $"P1: {score}";
            else
                scoreLbP2.Text = $"P2: {score}";
        }

        private void UpdateTime(int timeRemaining)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateTime(timeRemaining)));
                return;
            }
            timeLb.Text = $"Time: {timeRemaining}s";
        }

        private void HandleGameFinished(bool player1Wins)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleGameFinished(player1Wins)));
                return;
            }

            if (player1Wins)
                FormManager.Instance.NavigateToWinScreen();
            else
                FormManager.Instance.NavigateToGameOver();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GameManager.Instance.StartGame(gridPanel);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            GameManager.Instance.OnScoreUpdated -= UpdateScore;
            GameManager.Instance.OnTimeUpdated -= UpdateTime;
            GameManager.Instance.OnGameFinished -= HandleGameFinished;
        }

        private void GameController_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameManager.Instance.OnScoreUpdated -= UpdateScore;
            GameManager.Instance.OnTimeUpdated -= UpdateTime;
            GameManager.Instance.OnGameFinished -= HandleGameFinished;
            
            FormManager.Instance.GoBack();
        }

        private void GameController_Load(object sender, EventArgs e)
        {
            // Xử lý khi form load
        }

        private void gridPanel_Paint(object sender, PaintEventArgs e)
        {
            // Xử lý vẽ panel nếu cần
        }

        private void timeLb_Click(object sender, EventArgs e)
        {
            // Xử lý click vào label thời gian nếu cần
        }
    }
}