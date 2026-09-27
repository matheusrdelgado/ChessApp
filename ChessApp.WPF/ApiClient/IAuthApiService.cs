using System;
using System.Threading;
using System.Threading.Tasks;
using ChessApp.WPF.Contracts;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Interface para o serviço de autenticação com a ChessApi externa.
    /// Gerencia login, registo, renovação de tokens (refresh) e estado de autenticação.
    /// </summary>
    public interface IAuthApiService
    {
        /// <summary>
        /// Evento disparado quando o estado de autenticação muda (login bem sucedido ou logout).
        /// </summary>
        event Action<bool>? AuthenticationStateChanged;

        /// <summary>
        /// Indica se o utilizador está autenticado no momento.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Nome do utilizador autenticado no momento, se houver.
        /// </summary>
        string? CurrentUsername { get; }

        /// <summary>
        /// Obtém o access token JWT atual em memória ou armazenamento seguro.
        /// </summary>
        string? GetAccessToken();

        /// <summary>
        /// Realiza o login junto à API (POST /api/v1/auth/login).
        /// </summary>
        Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Regista um novo utilizador junto à API (POST /api/v1/auth/register).
        /// </summary>
        Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Renova o access token utilizando o refresh token guardado (POST /api/v1/auth/refresh).
        /// </summary>
        Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Termina a sessão atual, limpando os tokens do armazenamento seguro.
        /// </summary>
        Task LogoutAsync();
    }
}
