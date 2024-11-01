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
            //gameManager.OnGameOver += OnGameOver;
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
    }
}