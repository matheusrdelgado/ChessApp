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

            // Regista clientes HTTP e serviços de API (AuthApiService, GameApiService, Tokens)
            services.AddChessApiClients(config =>
            {
                config.BaseUrl = "http://localhost:5000/";
                config.TimeoutSeconds = 10;
            });

            return services.BuildServiceProvider();
        }
    }
}
