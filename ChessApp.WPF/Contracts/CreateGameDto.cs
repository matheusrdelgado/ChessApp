using System.Text.Json.Serialization;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Dados enviados para criar um novo registo de partida na API.
    /// Suporta modos: Local, PvE-Stockfish e PvP-Online.
    /// </summary>
    public class CreateGameDto
    {
        [JsonPropertyName("gameMode")]
        public string GameMode { get; set; } = "Local"; // "Local", "PvE-Stockfish", "PvP-Online"

        [JsonPropertyName("playerColor")]
        public string PlayerColor { get; set; } = "White"; // "White", "Black"

        [JsonPropertyName("difficulty")]
        public string? Difficulty { get; set; }

        [JsonPropertyName("opponent")]
        public string? Opponent { get; set; }

        public CreateGameDto() { }

        public CreateGameDto(string gameMode, string playerColor, string? difficulty = null, string? opponent = null)
        {
            GameMode = gameMode;
            PlayerColor = playerColor;
            Difficulty = difficulty;
            Opponent = opponent;
        }
    }
}
