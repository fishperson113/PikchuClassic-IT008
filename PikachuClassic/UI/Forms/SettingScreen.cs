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
    public partial class SettingScreen : Form
    {
        public SettingScreen()
        {
            InitializeComponent();
            this.FormClosed += FormChild_FormClosed;  // Đăng ký sự kiện FormClosed
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void FormChild_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            AudioManager.Instance.ToggleBackgroundMusic();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
