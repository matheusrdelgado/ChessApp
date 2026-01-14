using ChessApp.Model.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChessApp.WPF.Views;

namespace ChessApp.WPF.ViewModel
{
    public class RegisterViewModel : BaseViewModel
    {
        private readonly UserService _userService;

        public event Action OnRequestClose;

        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public ICommand RegisterCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public RegisterViewModel()
        {
            _userService = new UserService();
            RegisterCommand = new RelayCommand(p => PerformRegister(p));
            CancelCommand = new RelayCommand(p => OnRequestClose?.Invoke());
        }

        /// <summary>
        /// faz o registo do utilizador
        /// </summary>
        /// <param name="parameter"></param>
        private void PerformRegister(object parameter)
        {
            var window = parameter as Views.RegisterWindow; // Aceder à janela de registo para obter as passwords
            if (window == null) return;

            string p1 = window.txtRegPass.Password;
            string p2 = window.txtRegConfirm.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(p1)) // Verifica se o username ou password estão vazios
            {
                MessageBox.Show("Missing username or password.");
                return;
            }

            if (p1 != p2) // Verifica se as passwords coincidem
            {
                MessageBox.Show("Passwords must match!");
                return;
            }

            if (_userService.Register(Username, p1)) // Tenta registar o utilizador
            {
                MessageBox.Show("Successfully registered. Please log in");
                _userService.SaveUsers();
                OnRequestClose?.Invoke();
            }
            else // Se o registo falhar, informa que o username já existe
            {
                MessageBox.Show("This username already exists.");
            }
        }


    }
}