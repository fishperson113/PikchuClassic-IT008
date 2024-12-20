using PikachuClassic.Forms;
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
            AudioManager.Instance.StartBackgroundMusic("Win");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        //Click Setting button
        private void button2_Click(object sender, EventArgs e)
        {
            // Ẩn các nút của form cha khi form con mở
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;

            // Tạo instance của SettingScreen
            SettingScreen settingScreen = new SettingScreen();

            // Thiết lập GameTutorial là form con (MDI child)
            settingScreen.MdiParent = this;

            // Đảm bảo khi form con đóng, nút của form cha sẽ hiển thị lại
            settingScreen.FormClosed += (s, args) =>
            {
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
            };

            settingScreen.Show(); // Hiển thị form Tutorial
        }


        public WinScreen(string gameMode)
        {
            InitializeComponent();
            this.gameMode = gameMode;
            AudioManager.Instance.StartBackgroundMusic("Win");
        }

        //Click Retry button
        private void button1_Click(object sender, EventArgs e)
        {

            GameForm gameController = new GameForm();
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
