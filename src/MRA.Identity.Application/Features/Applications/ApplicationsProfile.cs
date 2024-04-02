using AutoMapper;
using MRA.Identity.Application.Contract.Applications.Commands;
using MRA.Identity.Application.Contract.Applications.Responses;

namespace MRA.Identity.Application.Features.Applications;

public class ApplicationsProfile : Profile
{
    public ApplicationsProfile()
    {
        CreateMap<CreateApplicationCommand, Domain.Entities.Application>();
        CreateMap<UpdateApplicationCommand, Domain.Entities.Application>();
        CreateMap<Domain.Entities.Application,UpdateApplicationCommand>();
        CreateMap<Domain.Entities.Application, ApplicationResponse>();
    }
}