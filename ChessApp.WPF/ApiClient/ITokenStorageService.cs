namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Contrato para armazenamento e recuperação segura de credenciais e tokens JWT.
    /// Isola o mecanismo de criptografia da lógica de negócio.
    /// </summary>
    public interface ITokenStorageService
    {
        /// <summary>
        /// Guarda os tokens e o username de forma encriptada.
        /// </summary>
        void SaveTokens(string accessToken, string refreshToken, string username);

        /// <summary>
        /// Obtém o access token encriptado guardado em disco.
        /// </summary>
        string? GetAccessToken();

        /// <summary>
        /// Obtém o refresh token encriptado guardado em disco.
        /// </summary>
        string? GetRefreshToken();

        /// <summary>
        /// Obtém o username associado à sessão atual guardada.
        /// </summary>
        string? GetSavedUsername();

        /// <summary>
        /// Limpa todos os tokens e credenciais guardadas (ex.: no logout).
        /// </summary>
        void ClearTokens();

        /// <summary>
        /// Indica se existe um access token guardado.
        /// </summary>
        bool HasToken { get; }
    }
}
