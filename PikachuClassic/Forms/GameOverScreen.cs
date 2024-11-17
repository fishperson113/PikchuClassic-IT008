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
            this.Close();
            MainMenu mainMenu = new MainMenu();
            mainMenu.Show();
        }


        public GameOverScreen(string gameMode)
        {
            InitializeComponent();
            this.gameMode = gameMode;
        }

        //Click Try Again button
        private void button1_Click(object sender, EventArgs e)
        {
            //this.Close();
            //GameController gameController = new GameController(gameMode); //Tạo trò chơi mới dựa trên chế độ chơi hiện tại
            //gameController.Show(); //Hiển thị màn hình chơi mới

            foreach (Form form in Application.OpenForms.Cast<Form>().ToList())
            {
                if (form is GameController && !form.IsDisposed)
                {
                    form.Close(); //Đóng tất cả các GameController đang mở
                }
            }

            GameController newGame = new GameController();
            newGame.Show();
            this.Close(); //Đóng GameOverScreen
        }

        private void GameOverScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit(); //Thoát toàn bộ ứng dụng khi form cuối cùng bị đóng
        }

    }
}
