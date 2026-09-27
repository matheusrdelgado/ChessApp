using System.ComponentModel.DataAnnotations;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Dados enviados no pedido de autenticação (Login).
    /// </summary>
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public LoginRequestDto() { }

        public LoginRequestDto(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
