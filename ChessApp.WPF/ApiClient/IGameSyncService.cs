using System;
using System.Threading;
using System.Threading.Tasks;
using ChessApp.WPF.Contracts;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Ponto de extensão para sincronização de partidas em tempo real (PvP Online).
    /// Isola a comunicação via WebSockets/SignalR Hub das ViewModels, permitindo
    /// adicionar o backend de tempo real futuramente sem alterar a UI ou a lógica de xadrez local.
    ///
    /// Fluxo previsto:
    /// Jogador A faz movimento local -> validado localmente -> SendMoveAsync(move) -> Hub ->
    /// MoveReceived disparado no cliente B -> aplicado no tabuleiro do Jogador B.
    /// </summary>
    public interface IGameSyncService
    {
        /// <summary>
        /// Disparado quando o adversário remoto realiza um movimento validado.
        /// </summary>
        event Action<MoveDto>? MoveReceived;

        /// <summary>
        /// Disparado quando um adversário remoto conecta-se à sala da partida.
        /// </summary>
        event Action<string>? PlayerJoined;

        /// <summary>
        /// Disparado quando o adversário remoto desconecta-se da partida.
        /// </summary>
        event Action<string>? PlayerDisconnected;

        /// <summary>
        /// Disparado quando o adversário remoto desiste da partida.
        /// </summary>
        event Action<string>? OpponentResigned;

        /// <summary>
        /// Disparado quando o estado da conexão em tempo real é alterado.
        /// </summary>
        event Action<bool>? ConnectionStateChanged;

        /// <summary>
        /// Indica se a conexão em tempo real com o Hub da partida está ativa.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Identificador da partida atualmente sincronizada.
        /// </summary>
        string? CurrentGameId { get; }

        /// <summary>
        /// Estabelece a ligação ao Hub de tempo real para uma partida específica.
        /// </summary>
        /// <param name="gameId">Identificador único da partida.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        Task ConnectAsync(string gameId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Encerra a ligação de tempo real com o Hub.
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Envia um movimento local validado para o adversário remoto via Hub.
        /// </summary>
        /// <param name="move">Dados do movimento efetuado.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        Task SendMoveAsync(MoveDto move, CancellationToken cancellationToken = default);

        /// <summary>
        /// Notifica o adversário remoto sobre a desistência da partida.
        /// </summary>
        /// <param name="reason">Motivo da desistência.</param>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        Task SendResignAsync(string reason, CancellationToken cancellationToken = default);
    }
}
