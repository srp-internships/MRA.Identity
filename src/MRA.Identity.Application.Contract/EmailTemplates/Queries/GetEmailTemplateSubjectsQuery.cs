using MediatR;
using MRA.Identity.Application.Contract.EmailTemplates.Responses;

namespace MRA.Identity.Application.Contract.EmailTemplates.Queries;

public class GetEmailTemplateSubjectsQuery : IRequest<List<EmailTemplateSubjectResponse>>;