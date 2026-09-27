using System.ComponentModel.DataAnnotations;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Dados enviados no pedido de registo de um novo utilizador.
    /// </summary>
    public class RegisterRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string? Email { get; set; }

        public RegisterRequestDto() { }

        public RegisterRequestDto(string username, string password, string? email = null)
        {
            Username = username;
            Password = password;
            Email = email;
        }
    }
}
