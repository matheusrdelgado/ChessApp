using System.Windows;
using ChessApp.Model.Model;
using ChessApp.WPF.ViewModel;

namespace ChessApp.WPF.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            var vm = new LoginViewModel();

            vm.OnRequestClose += (isSuccess) =>
            {
                DialogResult = isSuccess;
                Close();
            };

            DataContext = vm;
        }

        public User LoggedUser => (DataContext as LoginViewModel)?.LoggedUser;
    }
}