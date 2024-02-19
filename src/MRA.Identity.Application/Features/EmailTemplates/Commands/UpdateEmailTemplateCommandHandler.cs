using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.EmailTemplates.Commands;

namespace MRA.Identity.Application.Features.EmailTemplates.Commands;

public class UpdateEmailTemplateCommandHandler(
    IApplicationDbContext context,
    ISlugService slugService,
    IMapper mapper)
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

        if (await context.EmailTemplates.AnyAsync(s => s.Name == request.Name, cancellationToken))
        {
            throw new ValidationException("Duplicate name after update");
        }

        mapper.Map(request, emailTemplate);
        emailTemplate.Slug = slugService.GenerateSlug(request.Name);
        await context.SaveChangesAsync(cancellationToken);
        return emailTemplate.Slug;
    }
}