using System;
using System.Windows.Forms;

namespace PikachuClassic
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            InitializeAudio();
            InitializeGame();
            
            Application.Run();
        }

        private static void InitializeAudio()
        {
            AudioManager.Instance.LoadSound("BgMusic", @"..\..\Audio\1-01. Opening.mp3");
            AudioManager.Instance.LoadSound("Bg", @"..\..\Audio\Background.mp3");
            AudioManager.Instance.LoadSound("Win", @"..\..\Audio\Win.mp3");
            AudioManager.Instance.LoadSound("Lose", @"..\..\Audio\Lose.mp3");
            AudioManager.Instance.LoadSound("Correct", @"..\..\Audio\Correct.mp3");
            AudioManager.Instance.LoadSound("Wrong", @"..\..\Audio\Wrong.mp3");
            AudioManager.Instance.StartBackgroundMusic("BgMusic");
        }

        private static void InitializeGame()
        {
            FormManager.Instance.Initialize(); // Khởi tạo FormManager
            FormManager.Instance.NavigateToMainMenu(); // Chuyển đến MainMenu
        }
    }
}
