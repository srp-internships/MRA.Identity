using MediatR;

namespace MRA.Identity.Application.Contract.Applications.Commands;

public class CreateApplicationCommand : IRequest<Guid>
{
    public required string Name { get; set; }
    public string Description { get; set; }
    public required string ClientSecret { get; set; }
    public string[] CallbackUrls { get; set; }
    public bool IsProtected { get; set; }
}