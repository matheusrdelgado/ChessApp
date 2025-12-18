using System.Windows;
using ChessApp.WPF.ViewModel;

namespace ChessApp.WPF.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            var vm = new RegisterViewModel();

            vm.OnRequestClose += () => Close();

            DataContext = vm;
        }
    }
}