using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Commands;

public class GetAllUsersByFilterCommand : PagedListQuery<UserResponse>
{
    public string Skills { get; set; }
    public Guid? ApplicationId { get; set; }
    public string ApplicationClientSecret { get; set; }
}