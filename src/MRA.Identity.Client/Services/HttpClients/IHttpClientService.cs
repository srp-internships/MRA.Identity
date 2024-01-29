namespace MRA.Identity.Client.Services.HttpClients;

public interface IHttpClientService
{
    Task<ApiResponse<T>> PostAsJsonAsync<T>(string url, object content);

    Task<ApiResponse<T>> GetAsJsonAsync<T>(string url, object content = null);

    Task<ApiResponse> GetAsync(string url);

    Task<ApiResponse<T>> PutAsJsonAsync<T>(string url, object content);

    Task<ApiResponse> DeleteAsync(string url);
}