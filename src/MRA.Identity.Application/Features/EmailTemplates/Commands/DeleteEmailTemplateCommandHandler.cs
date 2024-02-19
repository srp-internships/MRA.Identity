using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.EmailTemplates.Commands;

namespace MRA.Identity.Application.Features.EmailTemplates.Commands;

public class DeleteEmailTemplateCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteEmailTemplateCommand, Unit>
{
    public async Task<Unit> Handle(DeleteEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var emailTemplate =
            await context.EmailTemplates.FirstOrDefaultAsync(e => e.Slug == request.Slug, cancellationToken);
        if (emailTemplate == null)
        {
            throw new NotFoundException($"The emailTemplate with slug {request.Slug} not found");
        }

        context.EmailTemplates.Remove(emailTemplate);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}