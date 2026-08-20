using InventoryApp.Application.Features.InventoryMovements.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.WebApi.Controllers;

[Route("api/movimientos-inventario")]
public class InventoryMovementsController : BaseApiController
{
    [HttpGet]
    [Authorize(Policy = "MovementsView")]
    public async Task<IActionResult> GetAll([FromQuery] GetMovementsByProductQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }
}
