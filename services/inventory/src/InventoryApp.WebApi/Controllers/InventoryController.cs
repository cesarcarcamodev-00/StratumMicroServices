using System.Security.Claims;
using InventoryApp.Application.Features.Inventory.Commands;
using InventoryApp.Application.Features.Inventory.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApp.WebApi.Controllers;

[Route("api/inventario")]
public class InventoryController : BaseApiController
{
    [HttpGet("stock/{productId:guid}")]
    [Authorize(Policy = "InventoryView")]
    public async Task<IActionResult> GetStock(Guid productId)
    {
        var result = await Mediator.Send(new GetProductStockQuery(productId));
        return Ok(result);
    }

    [HttpGet("stock-bajo")]
    [Authorize(Policy = "InventoryView")]
    public async Task<IActionResult> GetLowStock()
    {
        var result = await Mediator.Send(new GetLowStockProductsQuery());
        return Ok(result);
    }

    [HttpPost("ajustar")]
    [Authorize(Policy = "StocksManage")]
    public async Task<IActionResult> AdjustStock([FromBody] AdjustStockCommand command)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        var result = await Mediator.Send(command with { CreatedBy = username });
        return Ok(result);
    }

    [HttpPost("entradas")]
    [Authorize(Policy = "StocksManage")]
    public async Task<IActionResult> CreateEntryBySku([FromBody] CreateStockEntryBySkuCommand command)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        var result = await Mediator.Send(command with { CreatedBy = username });

        if (result.Succeeded)
            return Ok(result);

        if (result.Message == "NOT_FOUND")
            return NotFound(result);

        if (result.Message == "INACTIVE")
            return Conflict(result);

        return BadRequest(result);
    }
}
