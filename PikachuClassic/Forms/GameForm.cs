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
    public partial class GameForm : Form
    {
        private string gameMode;
        private List<Node> currentPath;
        private GameTutorial gameTutorial;
        public GameForm()
        {
            InitializeComponent();
            RegisterEvents();
            
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | 
                         ControlStyles.AllPaintingInWmPaint | 
                         ControlStyles.UserPaint, true);
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
            gameTutorial?.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GameManager.Instance.StartGame(gridPanel);
            GridManager.Instance.SetGameController(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            GameManager.Instance.OnScoreUpdated -= UpdateScore;
            GameManager.Instance.OnTimeUpdated -= UpdateTime;
            GameManager.Instance.OnGameFinished -= HandleGameFinished;
            gameTutorial?.Close();
        }

        private void GameController_FormClosed(object sender, FormClosedEventArgs e)
        {
            GameManager.Instance.OnScoreUpdated -= UpdateScore;
            GameManager.Instance.OnTimeUpdated -= UpdateTime;
            GameManager.Instance.OnGameFinished -= HandleGameFinished;
            gameTutorial?.Close();
            FormManager.Instance.GoBack();
        }

        private void GameController_Load(object sender, EventArgs e)
        {
            // Xử lý khi form load
        }

        private void gridPanel_Paint(object sender, PaintEventArgs e)
        {
            if (currentPath != null && currentPath.Count >= 2)
            {
                using (Pen pen = new Pen(Color.Red, 3))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    
                    for (int i = 0; i < currentPath.Count - 1; i++)
                    {
                        Point start = GetCenterPoint(currentPath[i].pictureBox);
                        Point end = GetCenterPoint(currentPath[i + 1].pictureBox);
                        e.Graphics.DrawLine(pen, start, end);
                    }
                }
            }
        }

        private Point GetCenterPoint(PictureBox pictureBox)
        {
            Point location = pictureBox.Location;
            int centerX = location.X + pictureBox.Width / 2;
            int centerY = location.Y + pictureBox.Height / 2;
            return new Point(centerX, centerY);
        }

        private void timeLb_Click(object sender, EventArgs e)
        {
            // Xử lý click vào label thời gian nếu cần
        }

        public void UpdatePath(List<Node> path)
        {
            currentPath = path;
            gridPanel.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (gameTutorial == null)
            {
                gameTutorial = new GameTutorial();
                gameTutorial.Show();

            }
            else
            {
                gameTutorial.Visible = !gameTutorial.Visible;
            }
        }
    }
}