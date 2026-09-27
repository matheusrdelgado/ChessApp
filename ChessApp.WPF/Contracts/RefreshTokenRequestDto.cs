using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Dados enviados para renovação automática do access token expirado.
    /// </summary>
    public class RefreshTokenRequestDto
    {
        [Required]
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        public RefreshTokenRequestDto() { }

        public RefreshTokenRequestDto(string token, string refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}
