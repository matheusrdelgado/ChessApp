using System;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Configurações globais de conexão com a ChessApi externa.
    /// Permite alterar o endereço base da API conforme o ambiente (local, staging, produção).
    /// </summary>
    public class ApiClientConfiguration
    {
        /// <summary>
        /// URL base da API externa. Por padrão aponta para o ambiente de desenvolvimento local.
        /// </summary>
        public string BaseUrl { get; set; } = "http://localhost:5000/";

        /// <summary>
        /// Timeout padrão para requisições HTTP (em segundos).
        /// </summary>
        public int TimeoutSeconds { get; set; } = 10;
    }
}
