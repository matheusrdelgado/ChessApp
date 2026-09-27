using System.Text.Json.Serialization;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Resposta devolvida pela API após o registo de utilizador.
    /// </summary>
    public class RegisterResponseDto
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("userId")]
        public string? UserId { get; set; }
    }
}
