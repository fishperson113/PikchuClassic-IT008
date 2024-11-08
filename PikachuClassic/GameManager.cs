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
        public event Action OnGameOver;
        #endregion
        #region Singleton
        private static GameManager instance;

        //Backul
        private GameController gameController;
        private static string gameMode;
        public static GameManager GetInstance(GameController controller, string mode)
        {
            if (instance == null || GameManager.gameMode != mode)
            {
                gameMode = mode;
                instance = new GameManager(controller, mode); // Khởi tạo với mode mới
            }
            return instance;
        }

        private GameManager(GameController controller, string mode) // Constructor nội bộ cho Singleton
        {
            this.gameController = controller;
            gameMode = mode; // Gán `gameMode` từ tham số mode
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

            // Khởi tạo Timer
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }

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
        private bool isPlayerTurn = false;
        #endregion
        public GameManager()
        {
            player1 = new Player();
            //player2 = new Player();
            player2 = new Bot(1);
            currentPlayer = player1; //Player 1 đi trước

            // Khởi tạo Timer
            timer = new Timer();
            timer.Interval = 1000; // Timer sẽ tick mỗi giây
            timer.Tick += Timer_Tick;
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
                //OnGameOver?.Invoke(); // Gọi sự kiện khi thời gian kết thúc
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
            SetPlayerTurn(isPlayerTurn);
            if (currentPlayer is Bot bot)
            {
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
            isPlayerTurn = !isPlayerTurn;
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
        public bool IsPlayerTurn()
        {
            return isPlayerTurn; // Trả về lượt hiện tại
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

            //string winner = GameManager.Instance.player1.Score > GameManager.Instance.player2.Score ? "Player 1" : "Player 2";
            //MessageBox.Show($"Chúc mừng! {winner} đã chiến thắng trò chơi!");

            //OnGameOver?.Invoke();

            bool isWin = player1.Score > player2.Score;
            gameController.EndGame(isWin);
        }
    }
}
