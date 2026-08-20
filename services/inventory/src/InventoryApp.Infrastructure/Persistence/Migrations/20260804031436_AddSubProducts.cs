using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSubProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubProductId",
                schema: "inventory",
                table: "Rules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                schema: "inventory",
                table: "InventoryMovements",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "SubProductId",
                schema: "inventory",
                table: "InventoryMovements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                schema: "inventory",
                table: "InventoryItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "SubProductId",
                schema: "inventory",
                table: "InventoryItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubProducts",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    CostCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    ReorderLevel = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 0m),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "inventory",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_SubProductId",
                schema: "inventory",
                table: "Rules",
                column: "SubProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryMovements_SubProductId",
                schema: "inventory",
                table: "InventoryMovements",
                column: "SubProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_SubProductId_Location",
                schema: "inventory",
                table: "InventoryItems",
                columns: new[] { "SubProductId", "Location" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubProducts_ProductId_Name",
                schema: "inventory",
                table: "SubProducts",
                columns: new[] { "ProductId", "Name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_SubProducts_SubProductId",
                schema: "inventory",
                table: "InventoryItems",
                column: "SubProductId",
                principalSchema: "inventory",
                principalTable: "SubProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryMovements_SubProducts_SubProductId",
                schema: "inventory",
                table: "InventoryMovements",
                column: "SubProductId",
                principalSchema: "inventory",
                principalTable: "SubProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_SubProducts_SubProductId",
                schema: "inventory",
                table: "Rules",
                column: "SubProductId",
                principalSchema: "inventory",
                principalTable: "SubProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_SubProducts_SubProductId",
                schema: "inventory",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_SubProducts_SubProductId",
                schema: "inventory",
                table: "InventoryMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_Rules_SubProducts_SubProductId",
                schema: "inventory",
                table: "Rules");

            migrationBuilder.DropTable(
                name: "SubProducts",
                schema: "inventory");

            migrationBuilder.DropIndex(
                name: "IX_Rules_SubProductId",
                schema: "inventory",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_SubProductId",
                schema: "inventory",
                table: "InventoryMovements");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_SubProductId_Location",
                schema: "inventory",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "SubProductId",
                schema: "inventory",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "SubProductId",
                schema: "inventory",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "SubProductId",
                schema: "inventory",
                table: "InventoryItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                schema: "inventory",
                table: "InventoryMovements",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                schema: "inventory",
                table: "InventoryItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
