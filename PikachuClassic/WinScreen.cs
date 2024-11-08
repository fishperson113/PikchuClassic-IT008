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

        private string gameMode;

        public WinScreen(string gameMode)
        {
            InitializeComponent();
            this.gameMode = gameMode;
        }

        //Click Retry button
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            // Khởi động lại
            ModeSelectionScreen modeSelectioneScreen = new ModeSelectionScreen();
            modeSelectioneScreen.Show();
            this.Hide();
        }

        //Click Home button
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
        }
    }
}
