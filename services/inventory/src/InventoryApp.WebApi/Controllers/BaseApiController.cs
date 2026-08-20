using InventoryApp.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected IActionResult Ok(Result result) => result.Succeeded ? base.Ok(result) : BadRequest(result);

    protected IActionResult Ok<T>(Result<T> result) => result.Succeeded ? base.Ok(result) : BadRequest(result);
}
