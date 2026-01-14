using ChessApp.Model.Model;
using ChessApp.Model.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChessApp.WPF.Views;

namespace ChessApp.WPF.ViewModel
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        public User LoggedUser { get; private set; }

        public event Action<bool> OnRequestClose; //tells view to close

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand OpenRegisterCommand { get; set; }

        public LoginViewModel()
        {
            _userService = new UserService();

            LoginCommand = new RelayCommand(p => PerformLogin(p));
            OpenRegisterCommand = new RelayCommand(p => OpenRegister());
        }

        /// <summary>
        /// Metodo para realizar login
        /// </summary>
        /// <param name="parameter"></param>
        private void PerformLogin(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Missing username or password.");
                return;
            }

            var user = _userService.Login(Username, password);

            if (user != null) // Login successful
            {
                LoggedUser = user;
                OnRequestClose?.Invoke(true); 
            }
            else // Login failed
            {
                MessageBox.Show("Incorrect username or password.");
            }
        }

        /// <summary>
        /// metodo para abrir a janela de registo
        /// </summary>
        private void OpenRegister()
        {
            var registerWin = new Views.RegisterWindow();
            registerWin.ShowDialog();
            _userService.LoadUsers();
        }
    }
}