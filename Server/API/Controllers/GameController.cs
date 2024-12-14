// Server/API/Controllers/GameController.cs
using Microsoft.AspNetCore.Mvc;
using Server.API.DTOs;
using Server.API.Enums;
using Server.Core.Interfaces;
using Server.Core.Services;
using System.Threading.Tasks;

namespace Server.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
        {
            var game = await _gameService.CreateGame(request.Mode);
            return Ok(game);
        }
        [HttpPost("{gameId}/join")]
        public async Task<IActionResult> JoinGame(string gameId, [FromBody] JoinGameRequest request)
        {
            var isReady = await _gameService.JoinGame(gameId, request.ConnectionId);
            return Ok(new { IsGameReady = isReady });
        }

        // Thêm endpoint để leave game
        [HttpPost("{gameId}/leave")]
        public async Task<IActionResult> LeaveGame(string gameId, [FromBody] LeaveGameRequest request)
        {
            await _gameService.LeaveGame(gameId, request.ConnectionId);
            return Ok();
        }
        [HttpPost("move")]
        public IActionResult MakeMove([FromBody] MoveDTO move)
        {
            var gameState = _gameService.MakeMove(move);
            return Ok(gameState);
        }

        [HttpGet("{gameId}")]
        public async Task<IActionResult> GetGameState(string gameId)
        {
            var gameState = await _gameService.GetGameState(gameId);
            return Ok(gameState);
        }
    }

    public class CreateGameRequest
    {
        public GameMode Mode { get; set; }
    }
    public class JoinGameRequest
    {
        public string ConnectionId { get; set; }
    }

    public class LeaveGameRequest
    {
        public string ConnectionId { get; set; }
    }
}