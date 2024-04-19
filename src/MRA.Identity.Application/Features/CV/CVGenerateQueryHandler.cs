using MediatR;
using MRA.Identity.Application.Common.Interfaces.DbContexts;
using MRA.Identity.Application.Common.Interfaces.Services;
using MRA.Identity.Application.Contract.CV;
using MRA.Identity.Application.Contract.Educations.Query;
using MRA.Identity.Application.Contract.Experiences.Query;
using MRA.Identity.Application.Contract.Profile.Queries;
using MRA.Identity.Application.Contract.Skills.Queries;
using MRA.Identity.Application.Services.GeneratePdfCV;
using QuestPDF.Fluent;


namespace MRA.Identity.Application.Features.CV;
public class CVGenerateQueryHandler(
    IApplicationDbContext dbContext,
    IUserHttpContextAccessor userHttpContext,
    IMediator mediator)
    : IRequestHandler<CVGenerateQuery, MemoryStream>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IUserHttpContextAccessor _userHttpContext = userHttpContext;

    public async Task<MemoryStream> Handle(CVGenerateQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await mediator.Send(new GetProfileQuery());
        var userSkills = await mediator.Send(new GetUserSkillsQuery());
        var userEducations = await mediator.Send(new GetEducationsByUserQuery());
        var userExperience = await mediator.Send(new GetExperiencesByUserQuery());


        InvoiceDocument document = new InvoiceDocument(userProfile, userSkills,
            userEducations, userExperience);

        var stream = new MemoryStream();
        document.GeneratePdf(stream);
        stream.Position = 0;
        return stream;

    }


}