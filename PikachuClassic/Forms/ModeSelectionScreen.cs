using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    public partial class ModeSelectionScreen : Form
    {
        public ModeSelectionScreen()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Click button PvP
        private void button1_Click(object sender, EventArgs e)
        {
            GetGameMode("PvP");
            StartGame("PvP");
        }

        //Click button PvE
        private void button2_Click(object sender, EventArgs e)
        {
            GetGameMode("PvE");
            StartGame("PvE");
        }

        //Hàm bắt đầu game
        private void StartGame(string gameMode)
        {
            this.Hide();
            GameController gameController = new GameController(gameMode);
            gameController.FormClosed += (s, args) => this.Show(); // Hiển thị lại màn hình chọn chế độ sau khi trò chơi đóng
            this.Hide();
            gameController.Show();
        }

        public string GetGameMode(string gameMode)
        {
            return gameMode;
        }
    }
}
