using ChessApp.Model.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;

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
                var fileService = new GameFileService();
                var gameLoaded = fileService.LoadGame(fileName);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"File: {fileName}");
                sb.AppendLine($"Total moves: {gameLoaded.MoveHistory.Count}");
                sb.AppendLine("-----------------------------");
                sb.AppendLine("Game Report:");

                int turnCount = 1;
                foreach (var move in gameLoaded.MoveHistory)
                {

                    if (move.PieceMoved != null && move.To != null)
                    {
                        sb.AppendLine($"{turnCount}. {move.PieceMoved.Color} {move.PieceMoved.PieceType} -> {move.Notation}");

                    }
                    turnCount++;
                }
            }
            catch (Exception ex)
            {
                MatchDetails = "Error reading file: " + ex.Message;
            }
        }

    }
}