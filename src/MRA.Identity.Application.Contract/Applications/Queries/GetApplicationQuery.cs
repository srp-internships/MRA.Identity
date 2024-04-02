using MediatR;
using MRA.Identity.Application.Contract.Applications.Responses;

namespace MRA.Identity.Application.Contract.Applications.Queries;

public class GetApplicationQuery : IRequest<ApplicationResponse>
{
    public required string Slug { get; set; }
}