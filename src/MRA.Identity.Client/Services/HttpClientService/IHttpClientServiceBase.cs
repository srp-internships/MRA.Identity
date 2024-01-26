﻿namespace MRA.Identity.Client.Services.HttpClientService
{
    public interface IHttpClientServiceBase
    {
        Task<ApiResponse<T>> PostAsJsonAsync<T>(string url, object content);

        Task<ApiResponse<T>> GetAsJsonAsync<T>(string url, object content = null);

        Task<ApiResponse<T>> PutAsJsonAsync<T>(string url, object content);

        Task<ApiResponse> DeleteAsync(string url);
    }
}
