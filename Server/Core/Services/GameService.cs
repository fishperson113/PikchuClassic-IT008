using Server.API.DTOs;
using Server.API.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Server.Core.Interfaces;
using Server.Core.Entities.Player;
using Server.Core.Entities.Game;
using System.Timers;
namespace Server.Core.Services
{
    public class GameService : IGameService
    {
        private readonly Dictionary<string, GameStateDTO> _games = new();
        private readonly GridManager _gridManager;
        private readonly Dictionary<int, Player> _players = new();
        private readonly Dictionary<string, System.Timers.Timer> _gameTimers = new();
        private readonly Dictionary<string, HashSet<string>> _gameConnections = new();
        public GridManager GridManager => _gridManager;
        public GameService()
        {
            // Initialize with desired grid size
            _gridManager = new GridManager(4, 5); // 4 rows, 5 cols example
        }
        public async Task<bool> JoinGame(string gameId, string connectionId)
        {
            if (!_games.ContainsKey(gameId))
                return false;

            if (!_gameConnections.ContainsKey(gameId))
                _gameConnections[gameId] = new HashSet<string>();

            _gameConnections[gameId].Add(connectionId);

            // Nếu đủ 2 người chơi, bắt đầu game
            if (_gameConnections[gameId].Count == 2)
            {
                _games[gameId].Status = GameStatus.InProgress;
                StartGameTimer(gameId);
                return true;
            }

            return false;
        }
        public async Task LeaveGame(string gameId, string connectionId)
        {
            if (_gameConnections.ContainsKey(gameId))
            {
                _gameConnections[gameId].Remove(connectionId);

                // Nếu còn ít hơn 2 người chơi, kết thúc game
                if (_gameConnections[gameId].Count < 2)
                {
                    await EndGame(gameId);
                }
            }
        }

        private async Task EndGame(string gameId)
        {
            if (_games.ContainsKey(gameId))
            {
                _games[gameId].Status = GameStatus.Completed;
                CleanupGame(gameId);
                _games.Remove(gameId);
                _gameConnections.Remove(gameId);
            }
        }
        private void StartGameTimer(string gameId)
        {
            var timer = new System.Timers.Timer(1000); // Tick mỗi giây
            timer.Elapsed += (s, e) => UpdateGameTime(gameId);
            timer.Start();
            _gameTimers[gameId] = timer;
        }

        private void UpdateGameTime(string gameId)
        {
            if (_games.TryGetValue(gameId, out var gameState))
            {
                gameState.RemainingTime--;

                if (gameState.RemainingTime <= 0)
                {
                    // Hết thời gian, chuyển lượt
                    gameState.CurrentTurn = gameState.CurrentTurn == 1 ? 2 : 1;
                    gameState.RemainingTime = 20; // Reset time
                }
            }
        }
        public async Task<GameDTO> CreateGame(GameMode mode)
        {
            string gameId = Guid.NewGuid().ToString();
            var grid = _gridManager.GetGrid();

            var gameState = new GameStateDTO
            {
                GameId = gameId,
                Grid = grid.Nodes,
                Scores = new Dictionary<int, int> { { 1, 0 }, { 2, 0 } },
                CurrentTurn = 1,
                RemainingTime = 20,
                Status = mode == GameMode.PvE ? GameStatus.InProgress : GameStatus.Waiting
            };
            _games[gameId] = gameState;

            if (mode == GameMode.PvE)
            {
                // Initialize Bot as Player 2
                _players[2] = new Bot(2,this);
                StartGameTimer(gameId);
            }
            else
            {
                // Initialize Player 2 as a real player
                _players[2] = new Player(2);
                _gameConnections[gameId] = new HashSet<string>();
            }


            return new GameDTO
            {
                GameId = gameId,
                Mode = mode,
                Grid = grid.Nodes,
                Players = new List<PlayerDTO> { /* Populate players */ },
                CreatedAt = DateTime.UtcNow
            };
        }
        public bool IsGameReady(string gameId)
        {
            return _gameConnections.ContainsKey(gameId) && _gameConnections[gameId].Count == 2;
        }
        private void CleanupGame(string gameId)
        {
            if (_gameTimers.TryGetValue(gameId, out var timer))
            {
                timer.Stop();
                timer.Dispose();
                _gameTimers.Remove(gameId);
                Console.WriteLine($"Game {gameId}: Timer cleaned up");
            }
        }
        public GameStateDTO MakeMove(MoveDTO move)
        {
            Console.WriteLine($"Server received move with mode: {move.Mode}");
            if (!_games.ContainsKey(move.GameId))
                throw new Exception("Game not found");

            var gameState = _games[move.GameId];

            if (move.Mode == GameMode.PvP && !IsGameReady(move.GameId))
            {
                throw new Exception("Waiting for players to join");
            }
            if (gameState.CurrentTurn != move.PlayerId)
            {
                Console.WriteLine($"Not player {move.PlayerId}'s turn");
                return gameState;
            }
            var grid = _gridManager.GetGrid();
            Console.WriteLine($"Making move: Player {move.PlayerId}, " +
                     $"From ({move.FirstNode.Row},{move.FirstNode.Column}) " +
                     $"To ({move.SecondNode.Row},{move.SecondNode.Column})");

            // Validate move
            Node firstNode = new Node(move.FirstNode.Row, move.FirstNode.Column, move.FirstNode.ImageId);
            Node secondNode = new Node(move.SecondNode.Row, move.SecondNode.Column, move.SecondNode.ImageId);

            if (firstNode.X < 0 || firstNode.X >= grid.Rows ||
                firstNode.Y < 0 || firstNode.Y >= grid.Cols ||
                secondNode.X < 0 || secondNode.X >= grid.Rows ||
                secondNode.Y < 0 || secondNode.Y >= grid.Cols)
            {
                Console.WriteLine("Invalid node coordinates");
                return gameState;
            }

            // Kiểm tra node không phải là ô trống
            if (grid.Nodes[firstNode.X, firstNode.Y] == 0 ||
                grid.Nodes[secondNode.X, secondNode.Y] == 0)
            {
                Console.WriteLine("Cannot select empty nodes");
                return gameState;
            }

            // Kiểm tra hai node khác nhau
            if (firstNode.X == secondNode.X && firstNode.Y == secondNode.Y)
            {
                Console.WriteLine("Cannot select the same node twice");
                return gameState;
            }

            if (/*_gridManager.HasPath(firstNode, secondNode) &&*/ grid.Nodes[firstNode.X, firstNode.Y] == grid.Nodes[secondNode.X, secondNode.Y])
            {
                // Remove matched nodes
                _gridManager.RemoveNodes(firstNode.X, firstNode.Y, secondNode.X, secondNode.Y);
                gameState.Grid = grid.Nodes;
                // Update scores
                var score = grid.GetScoreForImage(firstNode.ImageId);
                gameState.Scores[move.PlayerId] += (int)score;

                // Check if game is over
                if (grid.AreAllPicturesMatched())
                {
                    gameState.Status = GameStatus.Completed;
                    CleanupGame(move.GameId);
                }

                // Switch turn
                gameState.CurrentTurn = gameState.CurrentTurn == 1 ? 2 : 1;
                gameState.RemainingTime = 20;

                _games[move.GameId] = gameState;

                // If PvE and it's Bot's turn, make Bot move
                if (gameState.Status != GameStatus.Completed &&
                    gameState.CurrentTurn == 2 &&
                    move.Mode == GameMode.PvE)
                {
                    var botTimer = new System.Timers.Timer(3000); // Delay 3 giây
                    botTimer.AutoReset = false; // Chỉ chạy một lần
                    botTimer.Elapsed += (sender, e) =>
                    {
                        var botMove = (_players[2] as Bot).MakeMove(gameState, grid);
                        if (botMove != null)
                        {
                            botMove.Mode = GameMode.PvE;
                            MakeMove(botMove);
                        }
                        botTimer.Dispose(); // Dọn dẹp timer
                    };
                    botTimer.Start();
                }
            }
            gameState.Grid = grid.Nodes;
            return gameState;
        }

        public async Task<GameStateDTO> GetGameState(string gameId)
        {
            if (!_games.ContainsKey(gameId))
                throw new Exception("Game not found");

            return _games[gameId];
        }
    }
}
