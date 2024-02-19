using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.EmailTemplates.Commands;

namespace MRA.Identity.Application.Features.EmailTemplates.Commands;

public class UpdateEmailTemplateCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateEmailTemplateCommand, string>
{
    public async Task<string> Handle(UpdateEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var emailTemplate =
            await context.EmailTemplates.FirstOrDefaultAsync(e => e.Slug == request.Slug, cancellationToken);
        if (emailTemplate == null)
        {
            throw new NotFoundException($"The emailTemplate with slug {request.Slug} not found");
        }

        emailTemplate.Text = request.Text ?? emailTemplate.Text;
        emailTemplate.Subject = request.Subject ?? emailTemplate.Subject;
        emailTemplate.Slug = request.Subject ?? emailTemplate.Slug;
        
        await context.SaveChangesAsync(cancellationToken);
        return emailTemplate.Slug;
    }
}