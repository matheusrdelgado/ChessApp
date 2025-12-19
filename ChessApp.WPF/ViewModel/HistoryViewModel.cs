using ChessApp.Model.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Text.Json;
using ChessApp.Model.Enums;

namespace ChessApp.WPF.ViewModel
{
    public class HistoryViewModel : BaseViewModel
    {
        public ObservableCollection<string> HistoryFiles { get; set; }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        private string _selectedFile;
        public string SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnPropertyChanged();
                LoadMatchDetails(_selectedFile);
            }
        }

        private string _matchDetails;
        public string MatchDetails
        {
            get { return _matchDetails; }
            set { _matchDetails = value; OnPropertyChanged(); }
        }

        public event Action OnRequestClose;

        public ICommand CloseCommand { get; set; }

        public HistoryViewModel(string username)
        {
            HistoryFiles = new ObservableCollection<string>();
            LoadHistory(username);

            CloseCommand = new RelayCommand(p => OnRequestClose?.Invoke());
        }

        private void LoadHistory(string username)
        {
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");

            if (Directory.Exists(folder))
            {
                var files = Directory.GetFiles(folder, $"{username}_*.json")
                                     .Select(f => Path.GetFileName(f))
                                     .ToList();

                if (files.Any())
                {
                    foreach (var file in files)
                    {
                        HistoryFiles.Add(file);
                    }
                    StatusMessage = ""; // clean message
                }
                else
                {
                    StatusMessage = "Match not found.";
                }
            }
            else
            {
                StatusMessage = "Match not found (Non existent directory).";
            }
        }

        private void LoadMatchDetails(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            try
            {
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");
                string filePath = Path.Combine(folder, fileName);

                if (!File.Exists(filePath)) return;

                string jsonContent = File.ReadAllText(filePath);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"File: {fileName}");
                sb.AppendLine("-----------------------------");
                sb.AppendLine("Game Report:");

                using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                {
                    JsonElement root = doc.RootElement; 

                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        sb.AppendLine($"Total moves: {root.GetArrayLength()}");
                        sb.AppendLine("");

                        int turnCount = 1;
                        foreach (JsonElement move in root.EnumerateArray())
                        {
                            string notation = "?";
                            if (move.TryGetProperty("Notation", out JsonElement notationEl))
                                notation = notationEl.GetString();

                            string pieceDesc = "Piece";
                            if (move.TryGetProperty("PieceMoved", out JsonElement pieceEl))
                            {
                                int colorInt = pieceEl.GetProperty("Color").GetInt32();
                                int typeInt = pieceEl.GetProperty("PieceType").GetInt32();

                                pieceDesc = $"{(Color)colorInt} {(PieceType)typeInt}";
                            }

                            sb.AppendLine($"{turnCount}. {pieceDesc} -> {notation}");
                            turnCount++;
                        }
                    }
                }

                MatchDetails = sb.ToString();
            }
            catch (Exception ex)
            {
                MatchDetails = "Error reading file: " + ex.Message;
            }
        }

    }
}