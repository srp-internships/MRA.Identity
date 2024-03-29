using MediatR;
using MRA.Identity.Application.Contract.Applications.Responses;

namespace MRA.Identity.Application.Contract.Applications.Queries;

public class GetApplicationsQuery : IRequest<List<ApplicationResponse>>;