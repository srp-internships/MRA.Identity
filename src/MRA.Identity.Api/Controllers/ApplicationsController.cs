using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Identity.Application.Contract.Applications.Commands;
using MRA.Identity.Application.Contract.Applications.Queries;

namespace MRA.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(ApplicationPolicies.SuperAdministrator)]
public class ApplicationsController(ISender mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateApplicationCommand command)
    {
        var response = await mediator.Send(command);
        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateApplicationCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete(string slug)
    {
        await mediator.Send(new DeleteApplicationCommand() { Slug = slug });
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetApplicationsQuery());
        return Ok(result);
    }
}