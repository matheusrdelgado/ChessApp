using System;
using System.Text.Json.Serialization;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Resumo de uma partida retornado pela API para exibição no histórico do utilizador.
    /// </summary>
    public class GameSummaryDto
    {
        [JsonPropertyName("gameId")]
        public string GameId { get; set; } = string.Empty;

        [JsonPropertyName("gameMode")]
        public string GameMode { get; set; } = string.Empty; // "Local", "PvE-Stockfish", "PvP-Online"

        [JsonPropertyName("result")]
        public string Result { get; set; } = string.Empty;

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty;

        [JsonPropertyName("whitePlayer")]
        public string? WhitePlayer { get; set; }

        [JsonPropertyName("blackPlayer")]
        public string? BlackPlayer { get; set; }

        [JsonPropertyName("playedAt")]
        public DateTime PlayedAt { get; set; }

        [JsonPropertyName("totalMoves")]
        public int TotalMoves { get; set; }

        [JsonPropertyName("finalFen")]
        public string? FinalFen { get; set; }
    }
}
