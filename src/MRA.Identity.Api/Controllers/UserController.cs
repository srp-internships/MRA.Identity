using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Queries.CheckUserDetails;
using MRA.Identity.Application.Contract.UserEmail.Commands;

namespace MRA.Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(ApplicationPolicies.Reviewer)]
public class UserController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllUsersByFilters byFilters)
    {
        var users = await mediator.Send(byFilters);
        return Ok(users);
    }
    
    [HttpGet("GetListUsers/ByFilter")]
    public async Task<IActionResult> GetListUsers([FromQuery] GetListUsersQuery query)
    {
        var users = await mediator.Send(query);
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