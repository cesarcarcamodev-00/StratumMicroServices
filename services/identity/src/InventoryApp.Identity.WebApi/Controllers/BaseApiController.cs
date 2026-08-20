using InventoryApp.Identity.Application.Common.Models;
using InventoryApp.Identity.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Identity.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    protected Guid CurrentUserId => User.GetUserId();
    protected string CurrentUsername => User.GetUsername();
    protected IList<string> CurrentRoles => User.GetRoles();

    protected IActionResult Ok(Result result) => result.Succeeded ? base.Ok(result) : BadRequest(result);

    protected IActionResult Ok<T>(Result<T> result) => result.Succeeded ? base.Ok(result) : BadRequest(result);
}
