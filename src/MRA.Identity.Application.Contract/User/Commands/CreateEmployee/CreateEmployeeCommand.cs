using MediatR;

namespace MRA.Identity.Application.Contract.User.Commands.CreateEmployee;

public class CreateEmployeeCommand : IRequest<Guid>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    private string _username;

    public string Username
    {
        get { return _username; }
        set { _username = value.Trim(); }
    }
    public string Password { get; set; } = "";

    public Guid ApplicationId { get; set; }
}