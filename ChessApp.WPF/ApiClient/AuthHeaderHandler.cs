using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace ChessApp.WPF.ApiClient
{
    /// <summary>
    /// Manipulador HTTP (DelegatingHandler) responsável por anexar automaticamente o cabeçalho
    /// 'Authorization: Bearer {token}' em todos os pedidos autenticados.
    /// Caso receba um status 401 (Unauthorized), tenta renovar o token via refresh token
    /// e reemitir o pedido original de forma transparente.
    /// </summary>
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ITokenStorageService _tokenStorage;
        private readonly Lazy<IAuthApiService> _authApiService;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public AuthHeaderHandler(ITokenStorageService tokenStorage, Func<IAuthApiService> authApiFactory)
        {
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
            _authApiService = new Lazy<IAuthApiService>(authApiFactory ?? throw new ArgumentNullException(nameof(authApiFactory)));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. Anexa o token atual se disponível
            string? token = _tokenStorage.GetAccessToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 2. Executa o pedido
            var response = await base.SendAsync(request, cancellationToken);

            // 3. Se receber 401 Unauthorized e tivermos um refresh token, tenta renovar
            if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(_tokenStorage.GetRefreshToken()))
            {
                await _refreshLock.WaitAsync(cancellationToken);
                try
                {
                    // Verifica se outro pedido paralelo já atualizou o token
                    string? freshToken = _tokenStorage.GetAccessToken();
                    if (!string.IsNullOrEmpty(freshToken) && freshToken != token)
                    {
                        // Já foi renovado por outro thread, retenta com o novo token
                        return await CloneAndRetryAsync(request, freshToken, cancellationToken);
                    }

                    // Tenta renovar o token com a API
                    var refreshResult = await _authApiService.Value.RefreshTokenAsync(cancellationToken);
                    if (refreshResult.Success && !string.IsNullOrEmpty(refreshResult.Data?.Token))
                    {
                        // Reenvia com o novo token renovado
                        return await CloneAndRetryAsync(request, refreshResult.Data.Token, cancellationToken);
                    }
                }
                finally
                {
                    _refreshLock.Release();
                }
            }

            return response;
        }

        /// <summary>
        /// Clona o HttpRequestMessage original para reenviá-lo com o novo token,
        /// visto que uma instância de HttpRequestMessage só pode ser enviada uma vez pelo HttpClient.
        /// </summary>
        private async Task<HttpResponseMessage> CloneAndRetryAsync(HttpRequestMessage originalRequest, string newToken, CancellationToken cancellationToken)
        {
            var newRequest = new HttpRequestMessage(originalRequest.Method, originalRequest.RequestUri);

            // Copia headers da requisição
            foreach (var header in originalRequest.Headers)
            {
                if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)) continue;
                newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Anexa novo Bearer token
            newRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);

            // Copia conteúdo, se houver
            if (originalRequest.Content != null)
            {
                var contentBytes = await originalRequest.Content.ReadAsByteArrayAsync(cancellationToken);
                newRequest.Content = new ByteArrayContent(contentBytes);

                foreach (var contentHeader in originalRequest.Content.Headers)
                {
                    newRequest.Content.Headers.TryAddWithoutValidation(contentHeader.Key, contentHeader.Value);
                }
            }

            return await base.SendAsync(newRequest, cancellationToken);
        }
    }
}
