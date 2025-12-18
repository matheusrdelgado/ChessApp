using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace ChessApp.WPF
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(string username)
        {
            InitializeComponent();
            LoadHistory(username);
        }

        private void LoadHistory(string username)
        {
            string folder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Saves");

            if (Directory.Exists(folder))
            {
                var files = Directory.GetFiles(folder, $"{username}_*.json")
                                     .Select(f => System.IO.Path.GetFileName(f))
                                     .ToList();

                ListHistory.ItemsSource = files;

                if (files.Count == 0)
                    ListHistory.Items.Add("Match not found.");
            }
            else
            {
                Directory.CreateDirectory(folder);
                ListHistory.Items.Add("Match not found.");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}