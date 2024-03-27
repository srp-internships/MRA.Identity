using MediatR;

namespace MRA.Identity.Application.Contract.Applications.Commands;

public class UpdateApplicationCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; }
    public required string ClientSecret { get; set; }
    public string[] CallbackUrls { get; set; }
    public bool IsProtected { get; set; }
}