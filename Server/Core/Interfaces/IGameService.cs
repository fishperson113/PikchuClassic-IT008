using Server.API.Enums;
using Server.API.DTOs;
namespace Server.Core.Interfaces
{
    public interface IGameService
    {
        Task<GameDTO> CreateGame(GameMode mode);
        GameStateDTO MakeMove(MoveDTO move);
        Task<GameStateDTO> GetGameState(string gameId);
        Task<bool> JoinGame(string gameId, string connectionId);
        Task LeaveGame(string gameId, string connectionId);
    }
}
