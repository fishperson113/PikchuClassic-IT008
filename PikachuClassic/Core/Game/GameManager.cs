// Client/Core/Game/GameManager.cs:path/to/file
using PikachuClassic.Infrastructure.Network.Enums;
using PikachuClassic.Infrastructure.Network.DTOs;
using PikachuClassic.Core.Models;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PikachuClassic.Core.Game
{
    public class GameManager
    {
        private static readonly Lazy<GameManager> _instance = new Lazy<GameManager>(() => new GameManager());

        public static GameManager Instance => _instance.Value;

        private readonly ApiClient _apiClient;
        private string _currentGameId;
        private GridModel _currentGrid;
        private Player _player1;
        private Player _player2;
        private bool _isPlayer1Turn;
        private GameMode _currentGameMode;
        private string _currentPlayerId;
        // Events for UI updates
        public event Action<int, Player> OnScoreUpdated;
        public event Action<int> OnTimeUpdated;
        public event Action OnGameFinished;
        public event Action<int[,]> OnGridUpdated;

        private GameManager()
        {
            _apiClient = ApiClient.Instance;
        }
        public async Task<bool> JoinGame(string playerId)
        {
            try
            {
                if (string.IsNullOrEmpty(_currentGameId))
                {
                    throw new Exception("No active game to join");
                }

                _currentPlayerId = playerId;
                var joinRequest = new JoinGameRequest { ConnectionId = playerId };
                var isGameReady = await _apiClient.JoinGame(_currentGameId, joinRequest);

                Console.WriteLine($"Joined game with ID: {_currentGameId}, Player ID: {playerId}, IsReady: {isGameReady}");

                // Nếu game sẵn sàng, bắt đầu poll game state
                if (isGameReady)
                {
                    await PollGameState();
                }

                return isGameReady;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"JoinGame error: {ex.Message}");
                throw;
            }
        }
        public async Task StartGame(GameMode gameMode)
        {
            try
            {
                _currentGameMode = gameMode;
                _player1 = new Player { Id = 1, Name = "Player1" };
                _player2 = gameMode == GameMode.PvE ?
                    new Player { Id = 2, Name = "Bot" } :
                    new Player { Id = 2, Name = "Player2" };

                var game = await _apiClient.CreateGame(gameMode);
                if (game == null || string.IsNullOrEmpty(game.GameId))
                {
                    throw new Exception("Failed to create game: Invalid response from server");
                }

                _currentGameId = game.GameId;
                Console.WriteLine($"Game created with ID: {_currentGameId}");

                // Cập nhật grid ngay khi nhận được
                if (game.Grid != null)
                {
                    OnGridUpdated?.Invoke(game.Grid);
                }

                _isPlayer1Turn = true;

                // Chỉ bắt đầu poll state nếu là PvE hoặc đã join game thành công
                if (gameMode == GameMode.PvE)
                {
                    await PollGameState();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"StartGame error: {ex.Message}");
                MessageBox.Show($"Error starting game: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task PollGameState()
        {
            bool initialRender = true;
            while (true)
            {
                try
                {
                    _currentGrid = await _apiClient.GetGameState(_currentGameId);
                    if (_currentGrid == null) break;

                    if (initialRender && _currentGrid.Status == GameStatus.InProgress)
                    {
                        OnGridUpdated?.Invoke(_currentGrid.Grid);
                        initialRender = false;
                        Console.WriteLine("Initial grid rendered");
                    }

                    OnTimeUpdated?.Invoke(_currentGrid.RemainingTime);

                    if (_currentGrid.Status == GameStatus.Completed)
                    {
                        OnGridUpdated?.Invoke(_currentGrid.Grid); // Cập nhật grid lần cuối
                        OnGameFinished?.Invoke();
                        break;
                    }

                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"PollGameState error: {ex.Message}");
                    MessageBox.Show($"Error fetching game state: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
            }
        }

        public async Task MakeMove(int row1, int col1, int row2, int col2)
        {
            try
            {
                if (_currentGameMode == GameMode.PvP && !IsPlayerTurn())
                {
                    Console.WriteLine("Warning: Not your turn");
                    return;
                }
                if (string.IsNullOrEmpty(_currentGameId))
                {
                    Console.WriteLine("Warning: No active game");
                    return;
                }
                // Kiểm tra điều kiện trước khi thực hiện nước đi
                if (_currentGrid == null || _currentGrid.Grid == null)
                {
                    Console.WriteLine("Warning: Game state is not initialized");
                    return;
                }
                // Kiểm tra tọa độ hợp lệ
                if (!IsValidPosition(row1, col1) || !IsValidPosition(row2, col2))
                {
                    Console.WriteLine("Warning: Invalid position");
                    return;
                }

                int imageId1 = _currentGrid.Grid[row1, col1];
                int imageId2 = _currentGrid.Grid[row2, col2];

                // Kiểm tra xem hai ô có cùng loại hình ảnh không
                if (imageId1 != imageId2)
                {
                    Console.WriteLine("Warning: Selected tiles don't match");
                    return;
                }

                MoveDTO move = new MoveDTO
                {
                    GameId = _currentGameId,
                    PlayerId = _isPlayer1Turn ? 1 : 2,
                    Mode = _currentGameMode,
                    FirstNode = new NodeDTO { Row = row1, Column = col1, ImageId = imageId1 },
                    SecondNode = new NodeDTO { Row = row2, Column = col2, ImageId = imageId2 }
                };
                Console.WriteLine($"Sending move: GameId={_currentGameId}, " +
                        $"Mode={_currentGameMode}, " +
                        $"PlayerId={(_isPlayer1Turn ? 1 : 2)}, " +
                        $"From=({row1},{col1}), " +
                        $"To=({row2},{col2})");
                // Gửi nước đi lên server
                var updatedGameState = await _apiClient.MakeMove(move);

                // Kiểm tra kết quả trả về
                if (updatedGameState == null)
                {
                    throw new Exception("Server returned null game state");
                }

                // Cập nhật trạng thái game
                _currentGrid = new GridModel(updatedGameState);

                // Thông báo UI cập nhật grid mới
                OnGridUpdated?.Invoke(_currentGrid.Grid);

                // Cập nhật điểm số và thời gian
                if (_currentGrid.Scores != null)
                {
                    foreach (var score in _currentGrid.Scores)
                    {
                        var player = score.Key == 1 ? _player1 : _player2;
                        if (player != null)
                        {
                            OnScoreUpdated?.Invoke(score.Value, player);
                        }
                    }
                }

                // Cập nhật thời gian
                OnTimeUpdated?.Invoke(_currentGrid.RemainingTime);

                // Kiểm tra kết thúc game
                if (_currentGrid.Status == GameStatus.Completed)
                {
                    Console.WriteLine("Game completed");
                    OnGameFinished?.Invoke();
                }
                else
                {
                    // Chuyển lượt nếu game chưa kết thúc
                    _isPlayer1Turn = !_isPlayer1Turn;
                    Console.WriteLine($"Turn changed to: Player {(_isPlayer1Turn ? 1 : 2)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MakeMove error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error making move: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm helper để kiểm tra tọa độ hợp lệ
        private bool IsValidPosition(int row, int col)
        {
            if (_currentGrid?.Grid == null) return false;
            return row >= 0 && row < _currentGrid.Grid.GetLength(0) &&
                   col >= 0 && col < _currentGrid.Grid.GetLength(1);
        }

        private void UpdateUI()
        {
            if (_currentGrid?.Scores == null) // Thêm kiểm tra này
            {
                Console.WriteLine("Warning: Scores not available");
                return;
            }

            foreach (var score in _currentGrid.Scores)
            {
                var player = score.Key == 1 ? _player1 : _player2;
                if (player != null) // Thêm kiểm tra này
                {
                    OnScoreUpdated?.Invoke(score.Value, player);
                }
            }

            OnTimeUpdated?.Invoke(_currentGrid.RemainingTime);
        }

        public string GetCurrentGameId()
        {
            return _currentGameId;
        }

        public GameMode GetGameMode()
        {
            return _currentGameMode;
        }

        public bool IsPlayerTurn()
        {
            return _isPlayer1Turn && _player1.Name != "Bot" || !_isPlayer1Turn && _player2.Name != "Bot";
        }
    }
}