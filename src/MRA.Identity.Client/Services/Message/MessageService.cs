using System.Net.Http;
using System.Net.Http.Json;
using MRA.Identity.Application.Contract.Messages.Commands;
using MRA.Identity.Application.Contract.Messages.Responses;
using MRA.Identity.Client.Services.HttpClientService;

namespace MRA.Identity.Client.Services.Message;

public class MessageService(IHttpClientService httpClient) : IMessageService
{
    public async Task<ApiResponse<List<GetMessageResponse>>> GetAllMessagesAsync()
    {
        var result = await httpClient.GetAsJsonAsync<List<GetMessageResponse>>("message");
        return result;
    }

    public async Task<ApiResponse<bool>> SendMessageAsync(SendMessageCommand command)
    {
        var result = await httpClient.PutAsJsonAsync<bool>("message", command);
        return result;
    }
}
