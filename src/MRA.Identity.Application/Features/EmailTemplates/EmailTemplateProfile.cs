using AutoMapper;
using MRA.Identity.Application.Contract.EmailTemplates.Commands;
using MRA.Identity.Application.Contract.EmailTemplates.Responses;
using MRA.Identity.Domain.Entities;

namespace MRA.Identity.Application.Features.EmailTemplates;

public class EmailTemplateProfile : Profile
{
    public EmailTemplateProfile()
    {
        CreateMap<CreateEmailTemplateCommand, EmailTemplate>();
        CreateMap<EmailTemplate, EmailTemplateResponse>();
        CreateMap<UpdateEmailTemplateCommand, EmailTemplate>();
    }
}