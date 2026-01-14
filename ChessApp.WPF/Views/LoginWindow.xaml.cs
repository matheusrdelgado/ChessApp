using System.Windows;
using ChessApp.Model.Model;
using ChessApp.WPF.ViewModel;

namespace ChessApp.WPF.Views
{
    public partial class LoginWindow : Window //partial class porque o XAML gera outra parte da classe
    {
        /// <summary>
        /// Construtor da janela de login
        /// </summary>
        public LoginWindow()
        {
            InitializeComponent();
            var vm = new LoginViewModel();

            vm.OnRequestClose += (isSuccess) =>
            {
                DialogResult = isSuccess;
                Close();
            };

            DataContext = vm; // Define o contexto de dados para a ViewModel de login
        }

        /// <summary>
        /// utilizador autenticado
        /// </summary>
        public User LoggedUser => (DataContext as LoginViewModel)?.LoggedUser;
    }
}