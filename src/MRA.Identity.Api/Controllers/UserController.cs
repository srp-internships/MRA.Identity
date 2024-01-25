using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Identity.Application.Contract.User.Queries;
using MRA.Identity.Application.Contract.User.Queries.CheckUserDetails;

namespace MRA.Identity.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(ApplicationPolicies.Administrator)]
public class UserController : ControllerBase
{
    private readonly ISender _mediator;

    public UserController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await _mediator.Send(new GetAllUsersQuery());
        return Ok(users);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Get([FromRoute] string key)
    {
        key = key.Replace("GetById?Id=", "");
        Guid id;
        
        if (Guid.TryParse(key, out id))
        {
            var query = new GetUserByUserIdQuery { Id = id };
            var user = await _mediator.Send(query);
            return Ok(user);
        }

        var query1 = new GetUserByUsernameQuery { UserName = key };
        var userResponse = await _mediator.Send(query1);
        return Ok(userResponse);
    }

    [HttpGet("GetByUserId")]
    public async Task<IActionResult> GetByUserId([FromQuery] Guid id)
    {
            var query = new GetUserByUserIdQuery { Id = id };
            var user = await _mediator.Send(query);
            return Ok(user);
    }
    
    [HttpGet("CheckUserDetails/{userName}/{phoneNumber}/{email}")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckUserDetails([FromRoute] string userName, [FromRoute] string phoneNumber,
        [FromRoute] string email)
    {
        var result = await _mediator.Send(new CheckUserDetailsQuery()
            { UserName = userName, PhoneNumber = phoneNumber, Email = email });
        return Ok(result);
    }
}