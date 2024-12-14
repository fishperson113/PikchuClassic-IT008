// Client/Infrastructure/Network/ApiClient.cs:path/to/file
using Newtonsoft.Json;
using PikachuClassic.Infrastructure.Network.Enums;
using PikachuClassic.Infrastructure.Network.DTOs;
using PikachuClassic.Core.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PikachuClassic
{
    public class ApiClient
    {
        private static readonly Lazy<ApiClient> _instance = new Lazy<ApiClient>(() => new ApiClient("http://localhost:5000")); // Thay đổi baseUrl nếu cần
        public static ApiClient Instance => _instance.Value;

        private readonly HttpClient _client;

        private ApiClient(string baseUrl)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<GameDTO> CreateGame(GameMode mode)
        {
            try
            {
                var request = new CreateGameRequest { Mode = mode };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("/api/game/create", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GameDTO>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error creating game.", ex);
            }
        }

        public async Task<GameStateDTO> MakeMove(MoveDTO move)
        {
            try
            {
                var json = JsonConvert.SerializeObject(move);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("/api/game/move", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GameStateDTO>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("Error making move.", ex);
            }
        }

        public async Task<GridModel> GetGameState(string gameId)
        {
            if (string.IsNullOrEmpty(gameId)) // Thêm kiểm tra này
            {
                throw new ArgumentException("GameId cannot be null or empty");
            }

            try
            {
                Console.WriteLine($"Requesting game state for ID: {gameId}"); // Thêm log
                var response = await _client.GetAsync($"/api/game/{gameId}");
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response received: {responseContent}"); // Thêm log

                response.EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<GridModel>(responseContent);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetGameState error: {ex.Message}"); // Thêm log
                throw new Exception($"Error fetching game state: {ex.Message}", ex);
            }
        }
        public async Task<bool> JoinGame(string gameId, JoinGameRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync($"/api/game/{gameId}/join", content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Join game response: {responseContent}");

                var result = JsonConvert.DeserializeObject<JoinGameResponse>(responseContent);
                return result.IsGameReady;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API JoinGame error: {ex.Message}");
                throw;
            }
        }
    }
    public class JoinGameResponse
    {
        public bool IsGameReady { get; set; }
    }
    public class JoinGameRequest
    {
        public string ConnectionId { get; set; }
    }
    public class CreateGameRequest
    {
        public GameMode Mode { get; set; }
    }
}