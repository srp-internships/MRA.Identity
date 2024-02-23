using MediatR;
using MRA.Configurations.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.UserEmail.Commands;

namespace MRA.Identity.Application.Features.UserEmail.Commands;

public class SendEmailCommandHandler(IEmailService emailService)
    : IRequestHandler<SendEmailCommand, Unit>
{
    public async Task<Unit> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        await emailService.SendEmailAsync(request.Receivers, request.Text, request.Subject);
        return Unit.Value;
    }
}