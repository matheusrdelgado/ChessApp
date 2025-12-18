using System.Windows;
using ChessApp.Model.Model;
using ChessApp.WPF.ViewModel;

namespace ChessApp.WPF.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(string username)
        {
            InitializeComponent();
            var vm = new HistoryViewModel(username);

            vm.OnRequestClose += () => Close();

            DataContext = vm;
        }
    }
}