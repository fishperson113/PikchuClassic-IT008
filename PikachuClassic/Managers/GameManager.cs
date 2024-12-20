﻿using System;
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
        public event Action<bool> OnGameFinished;
        #endregion
        #region Singleton
        private static GameManager instance;

        //Backul
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
        private Player player1;
        private Player player2;
        private Player currentPlayer;
        #endregion
        GameManager()
        {
            // Khởi tạo Timer
            timer = new Timer();
            timer.Interval = 1000; // Timer sẽ tick mỗi giây
            timer.Tick += Timer_Tick;
        }
        public void Initialize(string mode)
        {
            if (mode == null) return;
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
        public void ResetData()
        {
            player1.ResetScore();
            player2.ResetScore();

            currentPlayer = player1;
            ResetTimer();
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
            OnScoreUpdated?.Invoke(player.Score, player);
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
            OnGameFinished?.Invoke(player1.Score > player2.Score);
        }
        public Player Player1 { get { return player1; } }
        public Player Player2 { get { return player2; } }
        public string GetGameMode()
        {
            return gameMode;
        }
        public void StartGame(Panel gamePanel)
        {
            if (player1 == null || player2 == null)
            {
                throw new InvalidOperationException("Game not initialized. Call Initialize() first.");
            }

            // Reset data và grid
            ResetData();
            GridManager.Instance.GenerateGrid(gamePanel);

            // Bắt đầu timer với 20s
            StartTimer(20);

            // Cập nhật UI ban đầu với điểm 0 cho cả hai player
            OnScoreUpdated?.Invoke(0, player1);
            OnScoreUpdated?.Invoke(0, player2);
            SetPlayerTurn(true); // Bắt đầu với player1
        }
    }
}
