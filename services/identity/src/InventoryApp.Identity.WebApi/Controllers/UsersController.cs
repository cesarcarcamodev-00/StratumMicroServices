using InventoryApp.Identity.Application.Features.Users.Commands;
using InventoryApp.Identity.Application.Features.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.Identity.WebApi.Controllers;

[Authorize(Policy = "ManageUsers")]
public class UsersController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null)
    {
        var result = await Mediator.Send(new GetUsersQuery { Page = page, PageSize = pageSize, Search = search });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await Mediator.Send(new GetUserByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
            return BadRequest("El id de la URL no coincide con el cuerpo de la petición");

        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        if (id == CurrentUserId)
            return BadRequest(new Application.Common.Models.Result { Succeeded = false, Errors = ["No puedes eliminar tu propio usuario"] });

        var result = await Mediator.Send(new DeleteUserCommand(id));
        return Ok(result);
    }
}
