using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

    }
}