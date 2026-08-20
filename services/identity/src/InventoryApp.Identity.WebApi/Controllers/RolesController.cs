using InventoryApp.Identity.Application.Features.Roles.Commands;
using InventoryApp.Identity.Application.Features.Roles.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Identity.WebApi.Controllers;

[Authorize(Policy = "ManageUsers")]
public class RolesController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetRoles([FromQuery] bool includeInactive = false)
    {
        var result = await Mediator.Send(new GetRolesQuery { IncludeInactive = includeInactive });
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la URL no coincide con el cuerpo de la petición");

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRole(Guid id)
    {
        var result = await Mediator.Send(new DeleteRoleCommand(id));
        return Ok(result);
    }
}
