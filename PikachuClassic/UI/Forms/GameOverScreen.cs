using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PikachuClassic.Core.Game;
using PikachuClassic.Infrastructure.Network.Enums;
using PikachuClassic.UI.Forms;
namespace PikachuClassic
{
    public partial class GameOverScreen : Form
    {
        private GameMode gameMode;

        public GameOverScreen(GameMode gameMode)
        {
            InitializeComponent();
            this.gameMode = gameMode;
            AudioManager.Instance.StartBackgroundMusic("Lose");
        }

        // Click Main Menu button
        private void button2_Click(object sender, EventArgs e)
        {
            FormManager.Instance.NavigateToMainMenu();
            this.Close();
        }

        // Click Try Again button
        private void button1_Click(object sender, EventArgs e)
        {
            FormManager.Instance.NavigateToGame(gameMode);
            this.Close();
        }

        private void GameOverScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Không cần gọi Application.Exit() vì FormManager quản lý vòng đời ứng dụng
            // Application.Exit(); // Có thể xóa dòng này
        }
    }
}
