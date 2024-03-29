using MediatR;

namespace MRA.Identity.Application.Contract.Applications.Commands;

public class CreateApplicationCommand : IRequest<string>
{
    public required string Name { get; set; }
    public string Description { get; set; }
    public string[] CallbackUrls { get; set; }
    public bool IsProtected { get; set; }
}