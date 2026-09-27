using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ChessApp.WPF.Contracts;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Implementação de IAuthApiService para comunicação com os endpoints de autenticação da ChessApi.
    /// Utiliza IHttpClientFactory para gerir instâncias de HttpClient com segurança e reutilização de sockets.
    /// </summary>
    public class AuthApiService : IAuthApiService
    {
        public const string HttpClientName = "ChessApiClient";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenStorageService _tokenStorage;

        private string? _currentUsername;
        private string? _currentAccessToken;

        public event Action<bool>? AuthenticationStateChanged;

        public bool IsAuthenticated => !string.IsNullOrEmpty(GetAccessToken());

        public string? CurrentUsername => _currentUsername ?? _tokenStorage.GetSavedUsername();

        public AuthApiService(IHttpClientFactory httpClientFactory, ITokenStorageService tokenStorage)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));

            // Tenta restaurar username da sessão anterior salva
            _currentUsername = _tokenStorage.GetSavedUsername();
            _currentAccessToken = _tokenStorage.GetAccessToken();
        }

        public string? GetAccessToken()
        {
            return _currentAccessToken ?? _tokenStorage.GetAccessToken();
        }

        public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(HttpClientName);
                var response = await client.PostAsJsonAsync("api/v1/auth/login", request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken);
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        _currentAccessToken = result.Token;
                        _currentUsername = string.IsNullOrEmpty(result.Username) ? request.Username : result.Username;

                        // Guarda de forma segura com DPAPI
                        _tokenStorage.SaveTokens(result.Token, result.RefreshToken, _currentUsername);
                        AuthenticationStateChanged?.Invoke(true);

                        return ApiResponse<LoginResponseDto>.Ok(result, "Autenticação realizada com sucesso.");
                    }

                    return ApiResponse<LoginResponseDto>.Fail("Resposta de autenticação inválida.", (int)response.StatusCode);
                }

                string errorDetails = await ReadErrorMessageAsync(response, "Utilizador ou palavra-passe incorretos.");
                return ApiResponse<LoginResponseDto>.Fail(errorDetails, (int)response.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<LoginResponseDto>.NetworkError($"Falha de ligação à API de autenticação: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return ApiResponse<LoginResponseDto>.NetworkError("O pedido de autenticação expirou (timeout).");
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.Fail($"Erro inesperado na autenticação: {ex.Message}");
            }
        }

        public async Task<ApiResponse<RegisterResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(HttpClientName);
                var response = await client.PostAsJsonAsync("api/v1/auth/register", request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<RegisterResponseDto>(cancellationToken: cancellationToken)
                                 ?? new RegisterResponseDto { Success = true, Message = "Registo efetuado com sucesso." };

                    return ApiResponse<RegisterResponseDto>.Ok(result, "Registo efetuado com sucesso.");
                }

                string errorDetails = await ReadErrorMessageAsync(response, "Não foi possível registar o utilizador. Verifique se o nome já existe.");
                return ApiResponse<RegisterResponseDto>.Fail(errorDetails, (int)response.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<RegisterResponseDto>.NetworkError($"Falha de ligação à API para registo: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return ApiResponse<RegisterResponseDto>.NetworkError("O pedido de registo expirou (timeout).");
            }
            catch (Exception ex)
            {
                return ApiResponse<RegisterResponseDto>.Fail($"Erro inesperado no registo: {ex.Message}");
            }
        }

        public async Task<ApiResponse<LoginResponseDto>> RefreshTokenAsync(CancellationToken cancellationToken = default)
        {
            string? currentToken = GetAccessToken();
            string? refreshToken = _tokenStorage.GetRefreshToken();

            if (string.IsNullOrEmpty(currentToken) || string.IsNullOrEmpty(refreshToken))
            {
                return ApiResponse<LoginResponseDto>.Fail("Nenhum token ou refresh token disponível para renovação.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient(HttpClientName);
                var request = new RefreshTokenRequestDto(currentToken, refreshToken);

                var response = await client.PostAsJsonAsync("api/v1/auth/refresh", request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken: cancellationToken);
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        _currentAccessToken = result.Token;
                        _tokenStorage.SaveTokens(result.Token, result.RefreshToken, _currentUsername ?? _tokenStorage.GetSavedUsername() ?? string.Empty);
                        return ApiResponse<LoginResponseDto>.Ok(result);
                    }
                }

                // Se o refresh falhar (ex: refresh token expirado ou revogado), limpa credenciais
                await LogoutAsync();
                return ApiResponse<LoginResponseDto>.Fail("A sessão expirou. Faça login novamente.", (int)response.StatusCode);
            }
            catch (Exception ex)
            {
                return ApiResponse<LoginResponseDto>.NetworkError($"Erro ao renovar sessão: {ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                string? token = GetAccessToken();
                if (!string.IsNullOrEmpty(token))
                {
                    var client = _httpClientFactory.CreateClient(HttpClientName);
                    // Opcional: notificar a API sobre o logout
                    using var request = new HttpRequestMessage(HttpMethod.Post, "api/v1/auth/logout");
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    await client.SendAsync(request);
                }
            }
            catch
            {
                // Falha de rede no logout não deve impedir a limpeza local
            }
            finally
            {
                _currentAccessToken = null;
                _currentUsername = null;
                _tokenStorage.ClearTokens();
                AuthenticationStateChanged?.Invoke(false);
            }
        }

        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string defaultMessage)
        {
            try
            {
                string content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    // Tenta extrair campo "message" ou "title" caso a API retorne ProblemDetails ou JSON
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("message", out var msgElement))
                    {
                        return msgElement.GetString() ?? defaultMessage;
                    }
                    if (doc.RootElement.TryGetProperty("title", out var titleElement))
                    {
                        return titleElement.GetString() ?? defaultMessage;
                    }
                }
            }
            catch
            {
                // Ignora falhas de parse de erro e usa mensagem padrão
            }

            return $"{defaultMessage} (HTTP {(int)response.StatusCode})";
        }
    }
}
