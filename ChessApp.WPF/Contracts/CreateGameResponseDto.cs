using System;
using System.Text.Json.Serialization;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Resposta devolvida pela API ao criar uma partida, contendo o identificador único gerado.
    /// </summary>
    public class CreateGameResponseDto
    {
        [JsonPropertyName("gameId")]
        public string GameId { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("status")]
        public string Status { get; set; } = "Active";
    }
}
