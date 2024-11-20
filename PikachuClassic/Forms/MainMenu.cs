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
            //SettingsScreen settingsScreen = new SettingsScreen();
            //settingsScreen.ShowDialog(); // Mở màn hình cài đặt
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
            FormManager.Instance.OpenForm(new ModeSelectionScreen());
        }

        private void MainMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); //Đảm bảo thoát ứng dụng khi MainMenu bị đóng
        }
    }

}
