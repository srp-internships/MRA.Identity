using MediatR;

namespace MRA.Identity.Application.Contract.Applications.Commands;

public class DeleteApplicationCommand : IRequest<Unit>
{
    public required string Slug { get; set; }
}