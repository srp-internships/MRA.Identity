using MediatR;
using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Identity.Application.Contract.EmailTemplates.Queries;

public class GetEmailTemplateNamesQuery : IRequest<List<EmailTemplateNamesResponse>>;