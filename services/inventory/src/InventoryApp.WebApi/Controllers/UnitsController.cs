using InventoryApp.Application.Features.UnitsOfMeasure.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.WebApi.Controllers;

[Route("api/unidades")]
public class UnitsController : BaseApiController
{
    [HttpGet]
    [Authorize(Policy = "InventoryView")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllUnitsQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }
}
