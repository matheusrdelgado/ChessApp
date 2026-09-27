using System;
using Microsoft.Extensions.DependencyInjection;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Métodos de extensão para configuração e registo dos serviços de API
    /// no contentor de injeção de dependências do .NET (IServiceCollection).
    /// </summary>
    public static class ApiClientServiceExtensions
    {
        public static IServiceCollection AddChessApiClients(
            this IServiceCollection services,
            Action<ApiClientConfiguration>? configure = null)
        {
            var config = new ApiClientConfiguration();
            configure?.Invoke(config);

            // Garante que a BaseUrl termina com '/'
            if (!config.BaseUrl.EndsWith("/"))
            {
                config.BaseUrl += "/";
            }

            services.AddSingleton(config);

            // Armazenamento seguro de tokens com DPAPI
            services.AddSingleton<ITokenStorageService, SecureTokenStorageService>();

            // Serviço de Autenticação
            services.AddSingleton<IAuthApiService, AuthApiService>();

            // Handler de autorização para adicionar Bearer Token e refresh automático em 401
            services.AddTransient(sp => new AuthHeaderHandler(
                sp.GetRequiredService<ITokenStorageService>(),
                () => sp.GetRequiredService<IAuthApiService>()
            ));

            // HttpClient para autenticação (sem handler de autenticação para evitar loop)
            services.AddHttpClient(AuthApiService.HttpClientName, client =>
            {
                client.BaseAddress = new Uri(config.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
            });

            // HttpClient autenticado para ciclo de vida de jogos e histórico
            services.AddHttpClient(GameApiService.AuthenticatedHttpClientName, client =>
            {
                client.BaseAddress = new Uri(config.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(config.TimeoutSeconds);
            }).AddHttpMessageHandler<AuthHeaderHandler>();

            // Serviço de partidas
            services.AddTransient<IGameApiService, GameApiService>();

            // Ponto de extensão para futuro SignalR PvP
            services.AddTransient<IGameSyncService, NoOpGameSyncService>();

            return services;
        }
    }
}
