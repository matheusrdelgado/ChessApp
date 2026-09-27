using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ChessApp.Model.Model;
using ChessApp.WPF.ApiClient;
using ChessApp.WPF.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ChessApp.WPF.ViewModel
{
    /// <summary>
    /// ViewModel responsável pelo ecrã de autenticação, comunicando com a API externa
    /// através de IAuthApiService e tratando eventuais falhas de conectividade.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private readonly IAuthApiService _authApiService;

        public User? LoggedUser { get; private set; }

        public event Action<bool>? OnRequestClose;

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand OpenRegisterCommand { get; }

        public LoginViewModel() : this(App.Services.GetRequiredService<IAuthApiService>())
        {
        }

        public LoginViewModel(IAuthApiService authApiService)
        {
            _authApiService = authApiService ?? throw new ArgumentNullException(nameof(authApiService));

            LoginCommand = new RelayCommand(async p => await PerformLoginAsync(p), _ => !IsBusy);
            OpenRegisterCommand = new RelayCommand(_ => OpenRegister(), _ => !IsBusy);
        }

        /// <summary>
        /// Realiza o login na API de forma assíncrona.
        /// Se a API estiver offline, oferece ao utilizador a opção de jogar no modo local.
        /// </summary>
        private async Task PerformLoginAsync(object? parameter)
        {
            var passwordBox = parameter as PasswordBox;
            var password = passwordBox?.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Preencha o utilizador e a palavra-passe.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsBusy = true;
            try
            {
                var response = await _authApiService.LoginAsync(new LoginRequestDto(Username.Trim(), password));

                if (response.Success && response.Data != null)
                {
                    // Utilizador autenticado com sucesso via JWT
                    LoggedUser = new User(response.Data.Username, string.Empty);
                    OnRequestClose?.Invoke(true);
                    return;
                }

                if (response.IsNetworkError)
                {
                    // Tratamento amigável de falha de rede sem bloquear o jogo local
                    var result = MessageBox.Show(
                        $"{response.Message}\n\nDeseja continuar no modo Convidado / Offline para jogar localmente?",
                        "Servidor Indisponível",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        LoggedUser = new User(Username.Trim(), string.Empty);
                        OnRequestClose?.Invoke(true);
                    }
                    return;
                }

                // Erro de credenciais ou validação
                MessageBox.Show(response.Message ?? "Utilizador ou palavra-passe incorretos.", "Erro de Login", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Abre a janela de registo de nova conta
        /// </summary>
        private void OpenRegister()
        {
            var registerWin = new Views.RegisterWindow();
            registerWin.ShowDialog();
        }
    }
}