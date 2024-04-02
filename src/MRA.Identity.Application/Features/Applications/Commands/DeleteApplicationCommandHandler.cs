using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Contract.Applications.Commands;

namespace MRA.Identity.Application.Features.Applications.Commands;

public class DeleteApplicationCommandHandler(IApplicationDbContext context)
    : IRequestHandler<DeleteApplicationCommand, Unit>
{
    public async Task<Unit> Handle(DeleteApplicationCommand request, CancellationToken cancellationToken)
    {
        var application =
            await context.Applications.FirstOrDefaultAsync(s => s.Slug == request.Slug, cancellationToken) ??
            throw new NotFoundException("Application with slug " + request.Slug + " not found");
        context.Applications.Remove(application);
        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}