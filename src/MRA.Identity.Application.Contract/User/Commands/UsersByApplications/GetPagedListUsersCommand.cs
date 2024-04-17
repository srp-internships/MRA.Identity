using MRA.Identity.Application.Contract.Common;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Application.Contract.User.Commands.UsersByApplications;

public class GetPagedListUsersCommand : PagedListQuery<UserResponse>
{
    public string Skills { get; set; }
    public Guid ApplicationId { get; set; }
    public string ApplicationClientSecret { get; set; }
}