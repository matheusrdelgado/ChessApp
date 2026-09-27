using System;
using System.Threading;
using System.Threading.Tasks;
using ChessApp.WPF.Contracts;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Implementação padrão (Null Object Pattern) de IGameSyncService.
    /// Utilizada em partidas locais ou contra o bot (PvE-Stockfish), ou enquanto
    /// o backend SignalR para PvP online não estiver ativo.
    /// Evita verificações repetitivas de null nas ViewModels.
    /// </summary>
    public class NoOpGameSyncService : IGameSyncService
    {
#pragma warning disable CS0067
        public event Action<MoveDto>? MoveReceived;
        public event Action<string>? PlayerJoined;
        public event Action<string>? PlayerDisconnected;
        public event Action<string>? OpponentResigned;
        public event Action<bool>? ConnectionStateChanged;
#pragma warning restore CS0067

        public bool IsConnected => false;
        public string? CurrentGameId => null;

        public Task ConnectAsync(string gameId, CancellationToken cancellationToken = default)
        {
            // Operação vazia no modo offline/PvE
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            return Task.CompletedTask;
        }

        public Task SendMoveAsync(MoveDto move, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task SendResignAsync(string reason, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
