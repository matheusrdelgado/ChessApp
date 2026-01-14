using System.Windows;
using ChessApp.Model.Model;
using ChessApp.WPF.ViewModel;

namespace ChessApp.WPF.Views
{
    public partial class HistoryWindow : Window //partial class porque o XAML gera outra parte da classe
    {
        /// <summary>
        /// Construtor da janela de histórico
        /// </summary>
        /// <param name="username"></param>
        public HistoryWindow(string username)
        {
            // Inicializa a janela de histórico
            InitializeComponent();
            var vm = new HistoryViewModel(username);

            vm.OnRequestClose += () => Close();

            DataContext = vm;
        }
    }
}