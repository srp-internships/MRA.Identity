using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MRA.Identity.Application.Common.Exceptions;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.Applications.Commands;

namespace MRA.Identity.Application.Features.Applications.Commands;

public class CreateApplicationCommandHandler(
    IApplicationDbContext context,
    IMapper mapper,
    ISlugService slugService,
    ICryptoStringService cryptoStringService)
    : IRequestHandler<CreateApplicationCommand, string>
{
    public async Task<string> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        var application = mapper.Map<Domain.Entities.Application>(request);
        var exist = await context.Applications.AnyAsync(s => s.Name.ToLower() == application.Name.ToLower(),
            cancellationToken);
        if (exist)
            throw new ValidationException("Application with name " + application.Name + " already exists");
        application.Slug = slugService.GenerateSlug(application.Name);
        application.ClientSecret = cryptoStringService.GetCryptoString();
        await context.Applications.AddAsync(application, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return application.Slug;
    }
}