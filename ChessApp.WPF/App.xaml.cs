using System;
using System.Windows;
using ChessApp.WPF.ApiClient;
using Microsoft.Extensions.DependencyInjection;

namespace ChessApp.WPF
{
    /// <summary>
    /// Ponto de entrada da aplicação WPF com configuração do contentor de injeção de dependências (IoC).
    /// </summary>
    public partial class App : Application
    {
        private static IServiceProvider? _services;

        /// <summary>
        /// Provedor global de serviços da aplicação para resolução limpa nas ViewModels.
        /// </summary>
        public static IServiceProvider Services => _services ??= ConfigureServices();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _services = ConfigureServices();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Lê URL base de variável de ambiente (CHESS_API_URL) ou usa padrão local
            string apiBaseUrl = Environment.GetEnvironmentVariable("CHESS_API_URL") ?? "http://localhost:5000/";

            // Regista clientes HTTP e serviços de API (AuthApiService, GameApiService, Tokens)
            services.AddChessApiClients(config =>
            {
                config.BaseUrl = apiBaseUrl;
                config.TimeoutSeconds = 10;
            });

            return services.BuildServiceProvider();
        }
    }
}
