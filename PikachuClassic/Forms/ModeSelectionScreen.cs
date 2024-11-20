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
        GameController gameController;
        public ModeSelectionScreen()
        {
            InitializeComponent();
            if(gameController == null)
            gameController = new GameController();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Click button PvP
        private void button1_Click(object sender, EventArgs e)
        {
            StartGame("PvP");
        }

        //Click button PvE
        private void button2_Click(object sender, EventArgs e)
        {
            StartGame("PvE");
        }

        //Hàm bắt đầu game
        private void StartGame(string gameMode)
        {
            this.Hide();
            gameController.SetGameMode(gameMode);
            FormManager.Instance.OpenForm(gameController);
        }
        private void ModeSelectionScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); //Thoát toàn bộ ứng dụng khi form cuối cùng bị đóng
        }

    }
}
