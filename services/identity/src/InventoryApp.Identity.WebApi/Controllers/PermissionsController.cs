using InventoryApp.Identity.Application.Features.Permissions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Identity.WebApi.Controllers;

[Authorize]
public class PermissionsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetPermissions([FromQuery] string? module = null)
    {
        var result = await Mediator.Send(new GetPermissionsQuery { Module = module });
        return Ok(result);
    }
}
