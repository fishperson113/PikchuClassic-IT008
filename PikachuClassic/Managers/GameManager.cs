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
        #region Timer
        private Timer timer;
        private int timeRemaining;
        private int turnTime;
        #endregion
        #region Events
        // Sự kiện để cập nhật điểm số lên giao diện
        public event Action<int, Player> OnScoreUpdated;
        public event Action<int> OnTimeUpdated;
        #endregion
        #region Singleton
        private static GameManager instance;

        //Backul
        private GameController gameController;
        private static string gameMode;

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
        #endregion
        #region Player
        //Player
        public Player player1;
        public Player player2;
        private Player currentPlayer;
        #endregion
        public GameManager()
        {
            // Khởi tạo Timer
            timer = new Timer();
            timer.Interval = 1000; // Timer sẽ tick mỗi giây
            timer.Tick += Timer_Tick;
        }
        public void Initialize(GameController controller, string mode)
        {
            if (gameController == null || gameMode != mode)
            {
                gameController = controller;
                gameMode = mode;

                if (gameMode == "PvP")
                {
                    player1 = new Player();
                    player2 = new Player();
                }
                else if (gameMode == "PvE")
                {
                    player1 = new Player();
                    player2 = new Bot(1);
                }
                currentPlayer = player1;
            }
        }

        #region Timer and Switch Turn

        // Hàm khởi tạo Timer với thời gian bắt đầu (giây)
        public void StartTimer(int seconds)
        {
            turnTime = seconds;
            ResetTimer();
            timer.Start();
        }

        // Hàm xử lý mỗi khi Timer "tick"

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeRemaining--; // Giảm thời gian còn lại

            // Gọi sự kiện để cập nhật thời gian còn lại lên giao diện
            OnTimeUpdated?.Invoke(timeRemaining);

            // Kiểm tra nếu hết thời gian
            if (timeRemaining <= 0)
            {
                SwitchTurn();
            }
        }
        public void ResetTimer()
        {
            // Đặt lại thời gian về thời gian cho mỗi lượt
            timeRemaining = turnTime;
            OnTimeUpdated?.Invoke(timeRemaining); // Cập nhật giao diện với thời gian mới
        }
        public async void SwitchTurn()
        {
            // Chuyển lượt giữa player1 và player2
            currentPlayer = (currentPlayer == player1) ? player2 : player1;
            ResetTimer();
            if (currentPlayer is Bot bot)
            {
                SetPlayerTurn(false);
                // Gọi MakeMove cho bot và chờ hoàn thành
                try
                {
                    await bot.MakeMove(GridManager.Instance);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Có lỗi xảy ra khi bot thực hiện nước đi: {ex.Message}");
                }
            }
            else
            {
                SetPlayerTurn(true);
            } 
                
        }
        #endregion
        public void AddScore(int points, Player player)
        {
            player.UpdateScore(points);
            OnScoreUpdated?.Invoke(player.Score, player); // Gọi sự kiện để cập nhật điểm trên UI
        }

        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }
        private void SetPlayerTurn(bool isPlayerTurn)
        {
            // Vô hiệu hóa hoặc kích hoạt lại các ô
            foreach (var pictureBox in GridManager.Instance.GetPictureBoxes())
            {
                pictureBox.Enabled = isPlayerTurn; // Cho phép thao tác nếu là lượt của Player
            }
        }

        public void CheckIfTheGameIsFinished()
        {
            if (!GridManager.Instance.Grid.AllPictureBoxesHidden()) return;

            timer.Stop();

            bool isWin = player1.Score > player2.Score;
            gameController.EndGame(isWin);
        }
    }
}
