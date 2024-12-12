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
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //Click Setting button
        private void button2_Click(object sender, EventArgs e)
        {
            FormManager.Instance.NavigateToSettings();
        }

        //Click Quit button
        private void button3_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to quit?", "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        //Click Play button
        private void button1_Click(object sender, EventArgs e)
        {
            FormManager.Instance.NavigateToModeSelection();
        }

        private void MainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); //Đảm bảo thoát ứng dụng khi MainMenu bị đóng
        }

        //Click Tutorial icon
        private void button4_Click(object sender, EventArgs e)
        {
            // Ẩn các nút của form cha khi form con mở
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;

            // Tạo instance của TutorialScreen
            GameTutorial gameTutorial = new GameTutorial();

            // Thiết lập GameTutorial là form con (MDI child)
            gameTutorial.MdiParent = this;

            // Đảm bảo khi form con đóng, nút của form cha sẽ hiển thị lại
            gameTutorial.FormClosed += (s, args) =>
            {
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                button4.Visible = true;
            };

            gameTutorial.Show(); // Hiển thị form Tutorial

        }

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    AudioManager.Instance.ToggleBackgroundMusic(); // Bật/Tắt nhạc nền
        //}
    }

}
