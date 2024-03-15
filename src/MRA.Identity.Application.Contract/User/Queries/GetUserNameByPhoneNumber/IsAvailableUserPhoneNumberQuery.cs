using MediatR;

namespace MRA.Identity.Application.Contract.User.Queries.GetUserNameByPhoneNumber;
public class IsAvailableUserPhoneNumberQuery : IRequest<bool>
{
    public string PhoneNumber { get; set; }
}
