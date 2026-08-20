using System;
using InventoryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitsAndSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "Products");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "SalesOrderLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "PurchaseOrderLines",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReorderLevel",
                table: "Products",
                type: "numeric(18,3)",
                precision: 18,
                scale: 3,
                nullable: false,
                defaultValue: 10m,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 10);

            migrationBuilder.CreateTable(
                name: "UnitsOfMeasure",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Symbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Dimension = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FactorToBase = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false, defaultValue: 1m),
                    IsBaseUnit = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitsOfMeasure", x => x.Id);
                });

            var seededAt = new DateTime(2026, 8, 2, 0, 0, 0, DateTimeKind.Utc);
            var units = SeedConstants.DefaultUnits();
            var unitColumns = new[] { "Id", "Name", "Symbol", "Dimension", "FactorToBase", "IsBaseUnit", "Description", "CreatedAt", "IsActive" };
            var unitValues = new object[units.Count, unitColumns.Length];
            for (var i = 0; i < units.Count; i++)
            {
                unitValues[i, 0] = units[i].Id;
                unitValues[i, 1] = units[i].Name;
                unitValues[i, 2] = units[i].Symbol;
                unitValues[i, 3] = units[i].Dimension;
                unitValues[i, 4] = units[i].FactorToBase;
                unitValues[i, 5] = units[i].IsBaseUnit;
                unitValues[i, 6] = units[i].Description;
                unitValues[i, 7] = seededAt;
                unitValues[i, 8] = true;
            }
            migrationBuilder.InsertData("UnitsOfMeasure", unitColumns, unitValues);

            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "String"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Group = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            var settingsColumns = new[] { "Id", "Key", "Value", "DataType", "Description", "Group", "CreatedAt", "IsActive" };
            var settingsValues = new object[,]
            {
                { Guid.Parse("40000000-0000-0000-0000-000000000001"), "Negocio.Nombre", "Mi Negocio", "String", "Nombre del negocio mostrado en la aplicación", "Negocio", seededAt, true },
                { Guid.Parse("40000000-0000-0000-0000-000000000002"), "Negocio.Moneda", "USD", "String", "Símbolo o código de la moneda", "Negocio", seededAt, true },
                { Guid.Parse("40000000-0000-0000-0000-000000000003"), "Inventario.DecimalesCantidad", "2", "Number", "Decimales permitidos para cantidades en órdenes", "Inventario", seededAt, true },
                { Guid.Parse("40000000-0000-0000-0000-000000000004"), "Inventario.AvisoStockBajo", "true", "Boolean", "Habilitar avisos de stock bajo", "Inventario", seededAt, true }
            };
            migrationBuilder.InsertData("AppSettings", settingsColumns, settingsValues);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE \"Products\" SET \"UnitId\" = '30000000-0000-0000-0000-000000000001' WHERE \"UnitId\" IS NULL");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "Products",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_UnitId",
                table: "Products",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                table: "AppSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitsOfMeasure_Name_Dimension",
                table: "UnitsOfMeasure",
                columns: new[] { "Name", "Dimension" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_UnitsOfMeasure_UnitId",
                table: "Products",
                column: "UnitId",
                principalTable: "UnitsOfMeasure",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_UnitsOfMeasure_UnitId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "UnitsOfMeasure");

            migrationBuilder.DropIndex(
                name: "IX_Products_UnitId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "SalesOrderLines",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "PurchaseOrderLines",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "ReorderLevel",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 10,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,3)",
                oldPrecision: 18,
                oldScale: 3,
                oldDefaultValue: 10m);

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "Products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
