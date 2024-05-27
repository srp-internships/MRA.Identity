using MRA.Identity.Application.Contract.Applications.Commands;
using MRA.Identity.Application.Contract.Applications.Responses;

namespace MRA.Identity.Client.Services.Applications;

public interface IApplicationsService
{
    Task<bool> PostAsync(CreateApplicationCommand command);
    Task<bool> PutAsync(UpdateApplicationCommand command);
    Task<bool> DeleteAsync(string slug);
    Task<List<ApplicationResponse>> GetAllAsync();
    Task<ApplicationResponse> GetAsync(string slug);
}