using System;
using System.Text.Json.Serialization;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Representa um lance (movimento) de xadrez serializável para a API e para o SignalR.
    /// </summary>
    public class MoveDto
    {
        [JsonPropertyName("moveNumber")]
        public int MoveNumber { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; } = string.Empty; // Ex: "e2"

        [JsonPropertyName("to")]
        public string To { get; set; } = string.Empty; // Ex: "e4"

        [JsonPropertyName("piece")]
        public string Piece { get; set; } = string.Empty; // Ex: "Pawn", "Knight"

        [JsonPropertyName("san")]
        public string? San { get; set; } // Notação algébrica padrão, ex: "e4", "Nf3"

        [JsonPropertyName("promotion")]
        public string? Promotion { get; set; } // "Queen", "Rook", etc.

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public MoveDto() { }

        public MoveDto(int moveNumber, string from, string to, string piece, string? san = null, string? promotion = null)
        {
            MoveNumber = moveNumber;
            From = from;
            To = to;
            Piece = piece;
            San = san;
            Promotion = promotion;
            Timestamp = DateTime.UtcNow;
        }
    }
}
