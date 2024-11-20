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
    public partial class GameOverScreen : Form
    {
        private string gameMode;
        public GameOverScreen()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        //Click Main Menu button
        private void button2_Click(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            FormManager.Instance.OpenForm(mainMenu);
        }


        public GameOverScreen(string gameMode)
        {
            InitializeComponent();
            this.gameMode = gameMode;
        }

        //Click Try Again button
        private void button1_Click(object sender, EventArgs e)
        {
            GameController gameController = new GameController();
            gameController.SetGameMode(GameManager.Instance.GetGameMode());  // Đảm bảo gameMode được thiết lập lại trong GameController
            FormManager.Instance.OpenForm(gameController);
        }

        private void GameOverScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); //Thoát toàn bộ ứng dụng khi form cuối cùng bị đóng
        }

    }
}
