using System.Text.Json.Serialization;

namespace ChessApp.WPF.Contracts
{
    /// <summary>
    /// Encapsula a resposta de operações com a API, indicando sucesso,
    /// dados de retorno ou eventuais falhas (incluindo erros de conectividade/offline).
    /// </summary>
    /// <typeparam name="T">Tipo do dado retornado.</typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public bool IsNetworkError { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                StatusCode = 200,
                IsNetworkError = false
            };
        }

        public static ApiResponse<T> Fail(string message, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message,
                StatusCode = statusCode,
                IsNetworkError = false
            };
        }

        public static ApiResponse<T> NetworkError(string message = "Não foi possível conectar à API. Verifique a sua conexão.")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message,
                StatusCode = 0,
                IsNetworkError = true
            };
        }
    }
}
