using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.WriteLine("Thư mục làm việc hiện tại: " + System.IO.Directory.GetCurrentDirectory());
            
            // Nạp tất cả âm thanh trước khi bắt đầu phát nhạc nền
            AudioManager.Instance.LoadSound("OpeningMusic", @"..\..\Audio\1-01. Opening.mp3");

            // Bắt đầu phát nhạc nền sử dụng soundName
            AudioManager.Instance.StartBackgroundMusic("OpeningMusic");

            FormManager.Instance.OpenForm(new MainMenu());
            Application.Run(); // Chạy ứng dụng
        }
    }
}
