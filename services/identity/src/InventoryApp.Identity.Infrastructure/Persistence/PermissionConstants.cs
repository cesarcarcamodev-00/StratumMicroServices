namespace InventoryApp.Identity.Infrastructure.Persistence;

public static class PermissionConstants
{
    // Identity / Users
    public const string UsersView = "identity.users.view";
    public const string UsersManage = "identity.users.manage";
    public const string RolesView = "identity.roles.view";
    public const string RolesManage = "identity.roles.manage";

    // Inventory module
    public const string InventoryView = "inventory.view";
    public const string InventoryProductsManage = "inventory.products.manage";
    public const string InventoryStocksManage = "inventory.stocks.manage";
    public const string InventoryMovementsView = "inventory.movements.view";
    public const string InventoryReportsView = "inventory.reports.view";
    public const string InventoryUnitsManage = "inventory.units.manage";
    public const string InventorySettingsManage = "inventory.settings.manage";
    public const string InventoryRulesManage = "inventory.rules.manage";

    public static IReadOnlyList<(string Name, string Module, string Description)> All = new List<(string, string, string)>
    {
        (UsersView, "Identity", "Ver lista de usuarios"),
        (UsersManage, "Identity", "Crear, editar y eliminar usuarios"),
        (RolesView, "Identity", "Ver roles y permisos"),
        (RolesManage, "Identity", "Crear, editar y eliminar roles"),
        (InventoryView, "Inventario", "Ver inventario y dashboard"),
        (InventoryProductsManage, "Inventario", "Crear y editar productos"),
        (InventoryStocksManage, "Inventario", "Registrar entradas, salidas y ajustes de stock"),
        (InventoryMovementsView, "Inventario", "Ver movimientos de inventario"),
        (InventoryReportsView, "Inventario", "Ver reportes"),
        (InventoryUnitsManage, "Inventario", "Gestionar unidades de medida"),
        (InventorySettingsManage, "Inventario", "Gestionar configuración del módulo"),
        (InventoryRulesManage, "Inventario", "Gestionar tipos de regla y reglas aplicadas")
    };
}
