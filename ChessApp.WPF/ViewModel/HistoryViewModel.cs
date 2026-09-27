using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using ChessApp.Model.Enums;
using ChessApp.WPF.ApiClient;
using ChessApp.WPF.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ChessApp.WPF.ViewModel
{
    /// <summary>
    /// ViewModel para consulta de histórico de partidas, obtendo os registos da API externa
    /// com fallback automático para os ficheiros locais (Saves/) caso a rede esteja indisponível.
    /// </summary>
    public class HistoryViewModel : BaseViewModel
    {
        private readonly IGameApiService _gameApiService;
        private readonly string _username;
        private readonly Dictionary<string, GameSummaryDto> _apiGamesMap = new();

        public ObservableCollection<string> HistoryFiles { get; } = new();

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private string _selectedFile = string.Empty;
        public string SelectedFile
        {
            get => _selectedFile;
            set
            {
                _selectedFile = value;
                OnPropertyChanged();
                DisplayMatchDetails(_selectedFile);
            }
        }

        private string _matchDetails = string.Empty;
        public string MatchDetails
        {
            get => _matchDetails;
            set { _matchDetails = value; OnPropertyChanged(); }
        }

        public event Action? OnRequestClose;
        public ICommand CloseCommand { get; }

        public HistoryViewModel(string username) 
            : this(username, App.Services.GetRequiredService<IGameApiService>())
        {
        }

        public HistoryViewModel(string username, IGameApiService gameApiService)
        {
            _username = username;
            _gameApiService = gameApiService ?? throw new ArgumentNullException(nameof(gameApiService));

            CloseCommand = new RelayCommand(_ => OnRequestClose?.Invoke());

            _ = LoadHistoryAsync(username);
        }

        /// <summary>
        /// Carrega o histórico da API e complementa com as partidas guardadas localmente em modo offline.
        /// </summary>
        private async Task LoadHistoryAsync(string username)
        {
            HistoryFiles.Clear();
            _apiGamesMap.Clear();

            bool apiLoaded = false;

            // 1. Tenta consultar o histórico na API
            try
            {
                var apiResponse = await _gameApiService.GetUserGameHistoryAsync(username);

                if (apiResponse.Success && apiResponse.Data != null && apiResponse.Data.Any())
                {
                    apiLoaded = true;
                    foreach (var game in apiResponse.Data)
                    {
                        string idShort = game.GameId.Length > 8 ? game.GameId.Substring(0, 8) : game.GameId;
                        string title = $"[Nuvem] {idShort} - {game.GameMode} ({game.Result}) - {game.PlayedAt:dd/MM/yyyy HH:mm}";
                        _apiGamesMap[title] = game;
                        HistoryFiles.Add(title);
                    }
                }
                else if (apiResponse.IsNetworkError)
                {
                    StatusMessage = "Servidor offline. A apresentar histórico guardado localmente.";
                }
            }
            catch
            {
                StatusMessage = "Falha ao contactar a API. A apresentar histórico local.";
            }

            // 2. Carrega partidas salvas em ficheiro local como fallback/complemento
            LoadLocalFiles(username);

            if (!HistoryFiles.Any())
            {
                StatusMessage = "Nenhuma partida encontrada.";
            }
            else if (apiLoaded && string.IsNullOrEmpty(StatusMessage))
            {
                StatusMessage = $"{HistoryFiles.Count} partida(s) carregada(s).";
            }
        }

        /// <summary>
        /// Lê ficheiros .json gravados na pasta local 'Saves'.
        /// </summary>
        private void LoadLocalFiles(string username)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
            if (!Directory.Exists(folder)) return;

            var files = Directory.GetFiles(folder, $"{username}_*.json")
                                 .Select(Path.GetFileName)
                                 .Where(f => !string.IsNullOrEmpty(f))
                                 .ToList();

            foreach (var file in files)
            {
                if (file != null && !HistoryFiles.Contains(file))
                {
                    HistoryFiles.Add($"[Local] {file}");
                }
            }
        }

        /// <summary>
        /// Exibe os detalhes da partida selecionada quer tenha vindo da API ou de ficheiro local.
        /// </summary>
        private void DisplayMatchDetails(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) return;

            // Caso seja uma partida da API externa
            if (_apiGamesMap.TryGetValue(item, out var game))
            {
                var sb = new StringBuilder();
                sb.AppendLine("=== Relatório de Partida (ChessApi) ===");
                sb.AppendLine($"ID da Partida: {game.GameId}");
                sb.AppendLine($"Modo de Jogo : {game.GameMode}");
                sb.AppendLine($"Resultado    : {game.Result}");
                sb.AppendLine($"Motivo       : {game.Reason}");
                sb.AppendLine($"Data / Hora  : {game.PlayedAt:dd/MM/yyyy HH:mm:ss}");
                sb.AppendLine($"Total Lances : {game.TotalMoves}");
                if (!string.IsNullOrEmpty(game.FinalFen))
                {
                    sb.AppendLine($"FEN Final    : {game.FinalFen}");
                }
                MatchDetails = sb.ToString();
                return;
            }

            // Caso seja um ficheiro local
            string localName = item.Replace("[Local] ", "").Trim();
            LoadLocalMatchDetails(localName);
        }

        /// <summary>
        /// Lê e formata os detalhes de uma partida salva localmente em JSON.
        /// </summary>
        private void LoadLocalMatchDetails(string fileName)
        {
            try
            {
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
                string filePath = Path.Combine(folder, fileName);

                if (!File.Exists(filePath)) return;

                string jsonContent = File.ReadAllText(filePath);
                var sb = new StringBuilder();
                sb.AppendLine($"Ficheiro Local: {fileName}");
                sb.AppendLine("-----------------------------");
                sb.AppendLine("Relatório de Lances:");

                using var doc = JsonDocument.Parse(jsonContent);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Array)
                {
                    sb.AppendLine($"Total de lances: {root.GetArrayLength()}");
                    sb.AppendLine("");

                    int turnCount = 1;
                    foreach (var move in root.EnumerateArray())
                    {
                        string notation = "?";
                        if (move.TryGetProperty("Notation", out var notationEl))
                            notation = notationEl.GetString() ?? "?";

                        string pieceDesc = "Peça";
                        if (move.TryGetProperty("PieceMoved", out var pieceEl))
                        {
                            int colorInt = pieceEl.GetProperty("Color").GetInt32();
                            int typeInt = pieceEl.GetProperty("PieceType").GetInt32();
                            pieceDesc = $"{(Color)colorInt} {(PieceType)typeInt}";
                        }

                        sb.AppendLine($"{turnCount}. {pieceDesc} -> {notation}");
                        turnCount++;
                    }
                }

                MatchDetails = sb.ToString();
            }
            catch (Exception ex)
            {
                MatchDetails = "Erro ao ler ficheiro local: " + ex.Message;
            }
        }
    }
}