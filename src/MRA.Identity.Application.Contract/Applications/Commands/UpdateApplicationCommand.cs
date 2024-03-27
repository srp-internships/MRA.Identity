using MediatR;

namespace MRA.Identity.Application.Contract.Applications.Commands;

public class UpdateApplicationCommand : IRequest<Unit>
{
    public required string Slug { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; }
    public string[] CallbackUrls { get; set; }
    public bool IsProtected { get; set; }
}