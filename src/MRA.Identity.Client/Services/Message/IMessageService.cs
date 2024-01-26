using MRA.Identity.Application.Contract.Messages.Commands;
using MRA.Identity.Application.Contract.Messages.Responses;

namespace MRA.Identity.Client.Services.Message;

public interface IMessageService
{
    Task<ApiResponse<bool>> SendMessageAsync(SendMessageCommand command);
    Task<ApiResponse<List<GetMessageResponse>>> GetAllMessagesAsync();
}
