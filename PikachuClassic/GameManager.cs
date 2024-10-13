using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace PikachuClassic
{
    public class GameManager
    {
        
        private int score = 0;
        private Timer timer;
        private int timeRemaining;
        // Sự kiện để cập nhật điểm số lên giao diện
        public event Action<int> OnScoreUpdated;
        public event Action<int> OnTimeUpdated;
        public event Action OnGameWon;
        public event Action OnGameOver;

        private static GameManager instance;
        public static GameManager Instance
        {
            get
            {
                // Kiểm tra và khởi tạo instance nếu chưa tồn tại
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }
        public GameManager()
        {
            score = 0;
        }
        // Hàm khởi tạo Timer với thời gian bắt đầu (giây)
        public void StartTimer(int seconds)
        {
            timeRemaining = seconds; // Đặt thời gian ban đầu

            timer = new Timer();
            timer.Interval = 1000; // Timer sẽ tick mỗi giây
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        // Hàm xử lý mỗi khi Timer "tick"
        private void Timer_Tick(object sender, EventArgs e)
        {
            timeRemaining--; // Giảm thời gian còn lại

            // Gọi sự kiện để cập nhật thời gian còn lại lên giao diện
            OnTimeUpdated?.Invoke(timeRemaining);

            // Kiểm tra nếu hết thời gian
            //if (timeRemaining <= 0)
            //{
            //    timer.Stop(); // Dừng timer
            //    OnGameLost?.Invoke(); // Gọi sự kiện khi thời gian kết thúc
            //}
        }
        public void AddScore(int points)
        {
            score += points;
            OnScoreUpdated?.Invoke(score); // Gọi sự kiện để cập nhật điểm trên UI
        }
    }
}
