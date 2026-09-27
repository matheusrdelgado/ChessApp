using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Dados enviados no fecho de uma partida para a API (PUT /api/v1/games/{id}/finish).
    /// </summary>
    public class FinishGameDto
    {
        [JsonPropertyName("gameId")]
        public string GameId { get; set; } = string.Empty;

        [JsonPropertyName("gameMode")]
        public string GameMode { get; set; } = "Local"; // "Local", "PvE-Stockfish", "PvP-Online"

        [JsonPropertyName("result")]
        public string Result { get; set; } = string.Empty; // "WhiteWin", "BlackWin", "Draw"

        [JsonPropertyName("reason")]
        public string Reason { get; set; } = string.Empty; // "Checkmate", "Resignation", "Timeout", "DrawAgreement"

        [JsonPropertyName("finalFen")]
        public string? FinalFen { get; set; }

        [JsonPropertyName("endedAt")]
        public DateTime EndedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("moves")]
        public List<MoveDto> Moves { get; set; } = new List<MoveDto>();

        public FinishGameDto() { }

        public FinishGameDto(string gameId, string gameMode, string result, string reason, List<MoveDto> moves, string? finalFen = null)
        {
            GameId = gameId;
            GameMode = gameMode;
            Result = result;
            Reason = reason;
            Moves = moves ?? new List<MoveDto>();
            FinalFen = finalFen;
            EndedAt = DateTime.UtcNow;
        }
    }
}
