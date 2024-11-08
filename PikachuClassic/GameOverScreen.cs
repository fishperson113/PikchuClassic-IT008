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
            this.Hide();
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
            this.Close();
        }

        private string gameMode;

        public GameOverScreen(string gameMode)
        {
            InitializeComponent();
            this.gameMode = gameMode;
        }
        //Click Try Again button
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            ModeSelectionScreen modeSelectioneScreen = new ModeSelectionScreen();
            modeSelectioneScreen.Show();
            this.Hide();
        }
    }
}
