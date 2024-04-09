using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Identity.Application.Contract.User.Commands.UsersByApplications;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Queries.CheckUserDetails;
using MRA.Identity.Application.Contract.UserEmail.Commands;

namespace MRA.Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(ApplicationPolicies.Administrator)]
public class UserController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetPagedListUsersQuery query)
    {
        var users = await mediator.Send(query);
        return Ok(users);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post([FromBody] GetPagedListUsersCommand command)
    {
        var users = await mediator.Send(new GetPagedListUsersQuery()
        {
            ApplicationId = command.ApplicationId,
            ApplicationClientSecret = command.ApplicationClientSecret,
            PageSize = command.PageSize,
            Skills = command.Skills,
            Page = command.Page,
            Sorts = command.Sorts,
            Filters = command.Filters
        });
        
        return Ok(users);
    }

    [HttpGet("GetListUsers/ByFilter")]
    public async Task<IActionResult> GetListUsers([FromQuery] GetListUsersQuery query)
    {
        var users = await mediator.Send(query);
        return Ok(users);
    }

    [HttpPost("GetListUsersCommand/ByFilter")]
    [AllowAnonymous]
    public async Task<IActionResult> GetListUsersCommand([FromBody] GetListUsersCommand command)
    {
        var users = await mediator.Send(new GetListUsersQuery()
        {
            ApplicationId = command.ApplicationId,
            ApplicationClientSecret = command.ApplicationClientSecret,
            Skills = command.Skills,
            FullName = command.FullName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber
        });
        return Ok(users);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey([FromRoute] string key)
    {
        var userResponse = await mediator.Send(new GetUserByKeyQuery { Key = key });
        return Ok(userResponse);
    }

    [HttpGet("CheckUserDetails/{userName}/{phoneNumber}/{email}")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckUserDetails([FromRoute] string userName, [FromRoute] string phoneNumber,
        [FromRoute] string email)
    {
        var result = await mediator.Send(new CheckUserDetailsQuery()
            { UserName = userName, PhoneNumber = phoneNumber, Email = email });
        return Ok(result);
    }

    [HttpPost("sendEmail")]
    public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
    {
        await mediator.Send(command);
        return Ok();
    }
}