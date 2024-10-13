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
            gameManager.OnScoreUpdated += UpdateScoreLabel;
            gameManager.OnTimeUpdated += UpdateTimeLabel;

            gameManager.StartTimer(60);
        }
        
        private void UpdateScoreLabel(int score)
        {
            scoreLb.Text = $"Score: {score}";
        }
        private void UpdateTimeLabel(int timeRemaining)
        {
            timeLb.Text = $"Time Left: {timeRemaining}";
        }
    }
}