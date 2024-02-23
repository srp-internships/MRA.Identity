using MediatR;

namespace MRA.Identity.Application.Contract.UserEmail.Commands;

public class SendEmailCommand : IRequest<Unit>
{
    public required IEnumerable<string> Receivers { get; set; }
    public required string Subject { get; set; }
    public required string Text { get; set; }
}