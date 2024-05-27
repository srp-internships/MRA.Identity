using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.EmailTemplates.Commands;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.EmailTemplates.Commands;

public class CreateEmailTemplateCommandHandler(
    IMapper mapper,
    IApplicationDbContext context,
    ISlugService slugService)
    : IRequestHandler<CreateEmailTemplateCommand, string>
{
    public async Task<string> Handle(CreateEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var slug = slugService.GenerateSlug(request.Name);
        if (await context.EmailTemplates.AnyAsync(e => e.Slug == slug, cancellationToken))
        {
            throw new ValidationException($"The template with name {request.Name} already exist");
        }

        var emailTemplate = mapper.Map<EmailTemplate>(request);
        emailTemplate.Slug = slug;
        await context.EmailTemplates.AddAsync(emailTemplate, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return slug;
    }
}