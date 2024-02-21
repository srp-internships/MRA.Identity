using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Queries.CheckUserDetails;
using MRA.Identity.Application.Contract.User.Responses;

namespace MRA.Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(ApplicationPolicies.Reviewer)]
public class UserController(ISender mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetAllUsersQueryByFilters queryByFilters)
    {
        var users = await mediator.Send(queryByFilters);
        return Ok(users);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKey([FromRoute] string key)
    {
        var userResponse = await mediator.Send(new GetUserByKeyQuery{Key = key});
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
}