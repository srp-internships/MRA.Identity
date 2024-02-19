using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRA.Identity.Application.Contract.EmailTemplates.Commands;
using MRA.Identity.Application.Contract.EmailTemplates.Queries;

namespace MRA.Identity.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmailTemplatesController(ISender mediator)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmailTemplateCommand createCommand)
    {
        var slug = await mediator.Send(createCommand);
        return Ok(slug);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string slug)
    {
        await mediator.Send(new DeleteEmailTemplateCommand { Slug = slug });
        return Ok();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateEmailTemplateCommand updateCommand)
    {
        var slug = await mediator.Send(updateCommand);
        return Ok(slug);
    }

    [HttpGet("getTemplate")]
    public async Task<IActionResult> GetTemplate([FromQuery] string slug)
    {
        var emailTemplateResponse = await mediator.Send(new GetEmailTemplateQuery { Slug = slug });
        return Ok(emailTemplateResponse);
    }

    [HttpGet("getSubjects")]
    public async Task<IActionResult> GetSubjects()
    {
        var emailTemplateSubjectResponses = await mediator.Send(new GetEmailTemplateSubjectsQuery());
        return Ok(emailTemplateSubjectResponses);
    }
}