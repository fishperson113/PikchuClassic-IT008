using Server.Core.Entities.Game;
using Server.API.DTOs;
using Server.Core.Services;
using Server.API.Enums;
namespace Server.Core.Entities.Player
{
    public class Bot : Player
    {
        private readonly GameService _gameService;
        private Random _random = new Random();

        public Bot(int id, GameService gameService) : base(id, "Bot")
        {
            _gameService = gameService;
        }

        public override MoveDTO MakeMove(GameStateDTO gameState, Grid grid)
        {
            // Implement Bot move logic here
            var unmatched = GetUnmatchedImages(grid);

            if (unmatched.Count < 2)
                return null;

            foreach (var imageId in unmatched.Keys)
            {
                var positions = unmatched[imageId];
                if (positions.Count >= 2)
                {
                    var node1 = new Node(positions[0].Row, positions[0].Column, imageId);
                    var node2 = new Node(positions[1].Row, positions[1].Column, imageId);

                    if (_gameService.GridManager.HasPath(node1, node2))
                    {
                        return new MoveDTO
                        {
                            GameId = gameState.GameId,
                            PlayerId = this.PlayerId,
                            FirstNode = new NodeDTO { Row = node1.X, Column = node1.Y, ImageId = node1.ImageId },
                            SecondNode = new NodeDTO { Row = node2.X, Column = node2.Y, ImageId = node2.ImageId },
                            Mode = GameMode.PvE
                        };
                    }
                }
            }

            // Fallback to random move
            var keys = new List<int>(unmatched.Keys);
            if (keys.Count < 1)
                return null;

            int selectedImage = keys[_random.Next(keys.Count)];
            var selectedPositions = unmatched[selectedImage];
            if (selectedPositions.Count >= 2)
            {
                return new MoveDTO
                {
                    GameId = gameState.GameId,
                    PlayerId = this.PlayerId,
                    FirstNode = new NodeDTO { Row = selectedPositions[0].Row, Column = selectedPositions[0].Column, ImageId = selectedImage },
                    SecondNode = new NodeDTO { Row = selectedPositions[1].Row, Column = selectedPositions[1].Column, ImageId = selectedImage }
                };
            }

            return null;
        }

        private Dictionary<int, List<(int Row, int Column)>> GetUnmatchedImages(Grid grid)
        {
            Dictionary<int, List<(int, int)>> unmatched = new Dictionary<int, List<(int, int)>>();

            for (int i = 0; i < grid.Rows; i++)
            {
                for (int j = 0; j < grid.Cols; j++)
                {
                    if (grid.Nodes[i, j] != 0)
                    {
                        if (!unmatched.ContainsKey(grid.Nodes[i, j]))
                            unmatched[grid.Nodes[i, j]] = new List<(int, int)>();

                        unmatched[grid.Nodes[i, j]].Add((i, j));
                    }
                }
            }

            return unmatched;
        }
    }
}
