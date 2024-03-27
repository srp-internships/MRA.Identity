using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.Applications.Commands;

namespace MRA.Identity.Application.Features.Applications.Commands;

public class UpdateApplicationCommandHandler(
    IApplicationDbContext context,
    IMapper mapper) : IRequestHandler<UpdateApplicationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateApplicationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Name cannot be empty");
        if (request.CallbackUrls.Any(s => !Uri.TryCreate(s, UriKind.Absolute, out _)))
            throw new ValidationException("Invalid callback url");
        //TODO: use fluentValidation

        if (await context.Applications.AnyAsync(
                s => s.Slug != request.Slug && s.Name.ToLower() == request.Name.ToLower(),
                cancellationToken))
            throw new ValidationException("Application with name" + request.Name + " already exists");
        var application =
            await context.Applications.FirstOrDefaultAsync(s => s.Slug == request.Slug, cancellationToken) ??
            throw new ValidationException("Application with slug " + request.Slug + " does not exist");
        ;


        mapper.Map(request, application);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}