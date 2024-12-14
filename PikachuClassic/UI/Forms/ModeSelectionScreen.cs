using PikachuClassic.Infrastructure.Network.Enums;
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
    public partial class ModeSelectionScreen : Form
    {
        public ModeSelectionScreen()
        {
            InitializeComponent();
        }

        private void ModeSelectionScreen_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormManager.Instance.GoBack();
        }

        private void btnPvP_Click(object sender, EventArgs e)
        {
            FormManager.Instance.NavigateToGame(GameMode.PvP);
        }

        private void btnPvE_Click(object sender, EventArgs e)
        {
            FormManager.Instance.NavigateToGame(GameMode.PvE);
        }
    }
}
