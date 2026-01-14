using System.Windows;
using ChessApp.WPF.ViewModel;

namespace ChessApp.WPF.Views
{
    public partial class RegisterWindow : Window //partial class porque o XAML gera outra parte da classe
    {
        /// <summary>
        /// logica de interacao para RegisterWindow.xaml
        /// </summary>
        public RegisterWindow()
        {
            InitializeComponent();
            var vm = new RegisterViewModel();

            vm.OnRequestClose += () => Close();

            DataContext = vm;
        }
    }
}