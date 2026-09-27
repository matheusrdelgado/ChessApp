using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Implementação segura de armazenamento de tokens utilizando a DPAPI (Data Protection API) do Windows.
    /// Os tokens são encriptados com chaves geradas pelo SO e atreladas à conta de utilizador do Windows atual,
    /// garantindo que ficheiros copiados para outro computador ou utilizador não possam ser lidos em texto claro.
    /// </summary>
    public class SecureTokenStorageService : ITokenStorageService
    {
        // Chave de entropia adicional para reforçar a segurança do DPAPI
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("ChessApp_WPF_SecureTokenVault_2026");

        private readonly string _storagePath;
        private readonly object _lock = new();

        private class TokenPayload
        {
            public string AccessToken { get; set; } = string.Empty;
            public string RefreshToken { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public DateTime SavedAt { get; set; } = DateTime.UtcNow;
        }

        public SecureTokenStorageService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = Path.Combine(appData, "CoreChess");
            Directory.CreateDirectory(appFolder);
            _storagePath = Path.Combine(appFolder, "auth_vault.dat");
        }

        public bool HasToken
        {
            get
            {
                lock (_lock)
                {
                    return !string.IsNullOrEmpty(GetAccessToken());
                }
            }
        }

        public void SaveTokens(string accessToken, string refreshToken, string username)
        {
            if (string.IsNullOrWhiteSpace(accessToken)) return;

            lock (_lock)
            {
                try
                {
                    var payload = new TokenPayload
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken ?? string.Empty,
                        Username = username ?? string.Empty,
                        SavedAt = DateTime.UtcNow
                    };

                    string json = JsonSerializer.Serialize(payload);
                    byte[] plainBytes = Encoding.UTF8.GetBytes(json);

                    // Encriptação com Windows DPAPI no escopo do utilizador do Windows atual
                    byte[] cipherBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser);

                    File.WriteAllBytes(_storagePath, cipherBytes);
                }
                catch
                {
                    // Falhas de I/O não devem quebrar a aplicação
                }
            }
        }

        public string? GetAccessToken()
        {
            return ReadPayload()?.AccessToken;
        }

        public string? GetRefreshToken()
        {
            return ReadPayload()?.RefreshToken;
        }

        public string? GetSavedUsername()
        {
            return ReadPayload()?.Username;
        }

        public void ClearTokens()
        {
            lock (_lock)
            {
                try
                {
                    if (File.Exists(_storagePath))
                    {
                        File.Delete(_storagePath);
                    }
                }
                catch
                {
                    // Ignora falha de deleção silenciosa
                }
            }
        }

        private TokenPayload? ReadPayload()
        {
            lock (_lock)
            {
                if (!File.Exists(_storagePath))
                    return null;

                try
                {
                    byte[] cipherBytes = File.ReadAllBytes(_storagePath);
                    if (cipherBytes.Length == 0) return null;

                    // Desencriptação com Windows DPAPI
                    byte[] plainBytes = ProtectedData.Unprotect(cipherBytes, Entropy, DataProtectionScope.CurrentUser);
                    string json = Encoding.UTF8.GetString(plainBytes);

                    return JsonSerializer.Deserialize<TokenPayload>(json);
                }
                catch
                {
                    // Se o ficheiro estiver corrompido ou não puder ser desencriptado, limpa-o
                    ClearTokens();
                    return null;
                }
            }
        }
    }
}
