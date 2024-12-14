using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PikachuClassic.Core.Game;
using PikachuClassic.Infrastructure.Network.Enums;
using PikachuClassic.UI.Forms;
namespace PikachuClassic
{
    public partial class WinScreen : Form
    {
        private GameMode gameMode;

        public WinScreen(GameMode gameMode)
        {
            InitializeComponent();
            this.gameMode = gameMode;
            AudioManager.Instance.StartBackgroundMusic("Win");
        }

        private void WinScreen_Load(object sender, EventArgs e)
        {
            // Logic khi form load nếu cần
        }

        // Click Setting button
        private void button2_Click(object sender, EventArgs e)
        {
            // Nếu bạn có SettingsScreen, hãy mở nó qua FormManager
            // FormManager.Instance.NavigateToSettings();
            // this.Close();
        }

        // Click Retry button
        private void button1_Click(object sender, EventArgs e)
        {
            FormManager.Instance.NavigateToGame(gameMode);
            this.Close();
        }

        // Click Home button
        private void button3_Click(object sender, EventArgs e)
        {
            FormManager.Instance.NavigateToMainMenu();
            this.Close();
        }

        private void WinScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Không cần gọi Application.Exit() vì FormManager quản lý vòng đời ứng dụng
            // Application.Exit(); // Có thể xóa dòng này
        }
    }
}
