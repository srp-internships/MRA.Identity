using System.Net.Http.Json;

namespace MRA.Identity.Client.Services.HttpClientService
{
    public static class Extensions
    {
        public static async Task<T> GetFromJsonAsync<T>(this HttpClient httpClient, string route, object query)
        {
            string queryString = string.Join("&", query.GetType().GetProperties()
                .Select(property => $"{property.Name}={property.GetValue(query)}"));
            return await httpClient.GetFromJsonAsync<T>($"{route}?{queryString}");

        }
    }
}
