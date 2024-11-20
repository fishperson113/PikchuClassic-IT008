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
    public partial class WinScreen : Form
    {
        private string gameMode;
        public WinScreen()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Click Setting button
        private void button2_Click(object sender, EventArgs e)
        {
            //SettingsScreen settingsScreen = new SettingsScreen();
            //settingsScreen.ShowDialog(); // Mở màn hình cài đặt
        }


        public WinScreen(string gameMode)
        {
            InitializeComponent();
            this.gameMode = gameMode;
        }

        //Click Retry button
        private void button1_Click(object sender, EventArgs e)
        {

            GameController gameController = new GameController();
            gameController.SetGameMode(GameManager.Instance.GetGameMode());  // Đảm bảo gameMode được thiết lập lại trong GameController
            FormManager.Instance.OpenForm(gameController);
        }

        //Click Home button
        private void button3_Click(object sender, EventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            FormManager.Instance.OpenForm(mainMenu);
        }

        private void GameWinScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); //Thoát toàn bộ ứng dụng khi form cuối cùng bị đóng
        }

    }
}
