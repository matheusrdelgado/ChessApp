using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ChessApp.WPF.ApiClient;
using ChessApp.WPF.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace ChessApp.WPF.ViewModel
{
    /// <summary>
    /// ViewModel responsável pelo registo de novos utilizadores via API externa.
    /// </summary>
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IAuthApiService _authApiService;

        public event Action? OnRequestClose;

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

        public ICommand RegisterCommand { get; }
        public ICommand CancelCommand { get; }

        public RegisterViewModel() : this(App.Services.GetRequiredService<IAuthApiService>())
        {
        }

        public RegisterViewModel(IAuthApiService authApiService)
        {
            _authApiService = authApiService ?? throw new ArgumentNullException(nameof(authApiService));

            RegisterCommand = new RelayCommand(async p => await PerformRegisterAsync(p), _ => !IsBusy);
            CancelCommand = new RelayCommand(_ => OnRequestClose?.Invoke(), _ => !IsBusy);
        }

        /// <summary>
        /// Efetua o registo de utilizador na API de forma assíncrona.
        /// </summary>
        private async Task PerformRegisterAsync(object? parameter)
        {
            var window = parameter as Views.RegisterWindow;
            if (window == null) return;

            string p1 = window.txtRegPass.Password;
            string p2 = window.txtRegConfirm.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(p1))
            {
                MessageBox.Show("Preencha o utilizador e a palavra-passe.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (p1 != p2)
            {
                MessageBox.Show("As palavras-passe não coincidem.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsBusy = true;
            try
            {
                var response = await _authApiService.RegisterAsync(new RegisterRequestDto(Username.Trim(), p1));

                if (response.Success)
                {
                    MessageBox.Show("Registo efetuado com sucesso! Por favor, inicie sessão.", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    OnRequestClose?.Invoke();
                    return;
                }

                if (response.IsNetworkError)
                {
                    MessageBox.Show(response.Message, "Falha de Ligação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                MessageBox.Show(response.Message ?? "Este nome de utilizador já se encontra registado.", "Erro de Registo", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}