using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await EnsureUnitsAsync(context);
    }

    private static async Task EnsureUnitsAsync(ApplicationDbContext context)
    {
        if (await context.UnitsOfMeasure.AnyAsync()) return;

        context.UnitsOfMeasure.AddRange(SeedConstants.DefaultUnits());
        await context.SaveChangesAsync();
    }
}
