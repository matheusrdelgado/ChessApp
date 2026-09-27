using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ChessApp.WPF.Contracts;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Interface para comunicação com a ChessApi relativamente ao ciclo de vida das partidas e histórico.
    /// </summary>
    public interface IGameApiService
    {
        /// <summary>
        /// Regista o início de uma nova partida na API e devolve o GameId gerado (POST /api/v1/games).
        /// </summary>
        Task<ApiResponse<CreateGameResponseDto>> CreateGameAsync(CreateGameDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Notifica a API sobre a finalização da partida, enviando resultado e histórico de lances (PUT /api/v1/games/{id}/finish).
        /// </summary>
        Task<ApiResponse<bool>> FinishGameAsync(FinishGameDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém o histórico de partidas de um determinado utilizador junto à API (GET /api/v1/games/user/{username}).
        /// </summary>
        Task<ApiResponse<List<GameSummaryDto>>> GetUserGameHistoryAsync(string username, CancellationToken cancellationToken = default);
    }
}
