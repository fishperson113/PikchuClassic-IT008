using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PikachuClassic.Infrastructure.Network.Enums;
using PikachuClassic.Infrastructure.Network.DTOs;
namespace PikachuClassic.Core.Models
{
    public class GridModel
    {
        public string GameId { get; set; }  // Thêm vào để đồng nhất với DTO
        public int[,] Grid { get; set; }
        public Dictionary<int, int> Scores { get; set; }
        public int CurrentTurn { get; set; }
        public int RemainingTime { get; set; }
        public GameStatus Status { get; set; }

        // Constructor để chuyển đổi từ DTO
        public GridModel(GameStateDTO dto)
        {
            GameId = dto.GameId;
            Grid = dto.Grid;
            Scores = dto.Scores;
            CurrentTurn = dto.CurrentTurn;
            RemainingTime = dto.RemainingTime;
            Status = dto.Status;
        }

        // Constructor mặc định
        public GridModel() { }

        // Các phương thức bổ sung cho logic game
        public bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < Grid.GetLength(0) &&
                   col >= 0 && col < Grid.GetLength(1);
        }

        public bool AreMatchingTiles(int row1, int col1, int row2, int col2)
        {
            if (!IsValidPosition(row1, col1) || !IsValidPosition(row2, col2))
                return false;

            return Grid[row1, col1] == Grid[row2, col2];
        }
    }
}
