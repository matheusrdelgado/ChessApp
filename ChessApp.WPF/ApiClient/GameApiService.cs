using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ChessApp.WPF.Contracts;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Implementação de IGameApiService para persistência e ciclo de vida de partidas na ChessApi.
    /// Chamadas utilizam HttpClientFactory com cabeçalho de autorização gerido automaticamente.
    /// Em caso de falha de conexão, devolve ApiResponse com status de erro sem travar o jogo local.
    /// </summary>
    public class GameApiService : IGameApiService
    {
        public const string AuthenticatedHttpClientName = "ChessApiAuthenticatedClient";

        private readonly IHttpClientFactory _httpClientFactory;

        public GameApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<ApiResponse<CreateGameResponseDto>> CreateGameAsync(CreateGameDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient(AuthenticatedHttpClientName);
                var response = await client.PostAsJsonAsync("api/v1/games", request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<CreateGameResponseDto>(cancellationToken: cancellationToken);
                    if (result != null && !string.IsNullOrEmpty(result.GameId))
                    {
                        return ApiResponse<CreateGameResponseDto>.Ok(result);
                    }

                    // Caso a API retorne apenas o ID em texto simples ou JSON alternativo
                    string rawId = await response.Content.ReadAsStringAsync(cancellationToken);
                    return ApiResponse<CreateGameResponseDto>.Ok(new CreateGameResponseDto { GameId = rawId.Trim('"') });
                }

                string error = await ReadErrorMessageAsync(response, "Não foi possível criar o registo da partida na API.");
                return ApiResponse<CreateGameResponseDto>.Fail(error, (int)response.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<CreateGameResponseDto>.NetworkError($"API indisponível: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return ApiResponse<CreateGameResponseDto>.NetworkError("Tempo limite de ligação excedido ao criar partida.");
            }
            catch (Exception ex)
            {
                return ApiResponse<CreateGameResponseDto>.Fail($"Erro ao iniciar partida na API: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> FinishGameAsync(FinishGameDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.GameId))
            {
                return ApiResponse<bool>.Fail("Identificador de partida inválido.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient(AuthenticatedHttpClientName);
                var response = await client.PutAsJsonAsync($"api/v1/games/{Uri.EscapeDataString(request.GameId)}/finish", request, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    return ApiResponse<bool>.Ok(true, "Partida reportada com sucesso.");
                }

                string error = await ReadErrorMessageAsync(response, "Falha ao reportar resultado da partida à API.");
                return ApiResponse<bool>.Fail(error, (int)response.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<bool>.NetworkError($"Falha de rede ao reportar fim de partida: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return ApiResponse<bool>.NetworkError("Timeout ao reportar fim de jogo à API.");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.Fail($"Erro ao reportar partida: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<GameSummaryDto>>> GetUserGameHistoryAsync(string username, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return ApiResponse<List<GameSummaryDto>>.Fail("Nome de utilizador não especificado.");
            }

            try
            {
                var client = _httpClientFactory.CreateClient(AuthenticatedHttpClientName);
                
                // Tenta rota /api/v1/games/user/{username} e rota alternativa /api/v1/games?username={username}
                string url = $"api/v1/games/user/{Uri.EscapeDataString(username)}";
                var response = await client.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    url = $"api/v1/games?username={Uri.EscapeDataString(username)}";
                    response = await client.GetAsync(url, cancellationToken);
                }

                if (response.IsSuccessStatusCode)
                {
                    var games = await response.Content.ReadFromJsonAsync<List<GameSummaryDto>>(cancellationToken: cancellationToken);
                    return ApiResponse<List<GameSummaryDto>>.Ok(games ?? new List<GameSummaryDto>());
                }

                string error = await ReadErrorMessageAsync(response, "Não foi possível carregar o histórico de jogos.");
                return ApiResponse<List<GameSummaryDto>>.Fail(error, (int)response.StatusCode);
            }
            catch (HttpRequestException ex)
            {
                return ApiResponse<List<GameSummaryDto>>.NetworkError($"API indisponível para consulta de histórico: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                return ApiResponse<List<GameSummaryDto>>.NetworkError("Tempo limite excedido ao carregar histórico.");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<GameSummaryDto>>.Fail($"Erro ao consultar histórico: {ex.Message}");
            }
        }

        private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, string defaultMessage)
        {
            try
            {
                string content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(content))
                {
                    using var doc = JsonDocument.Parse(content);
                    if (doc.RootElement.TryGetProperty("message", out var msg))
                        return msg.GetString() ?? defaultMessage;
                    if (doc.RootElement.TryGetProperty("title", out var title))
                        return title.GetString() ?? defaultMessage;
                }
            }
            catch
            {
                // Ignora falhas de parse de erro
            }

            return $"{defaultMessage} (HTTP {(int)response.StatusCode})";
        }
    }
}
