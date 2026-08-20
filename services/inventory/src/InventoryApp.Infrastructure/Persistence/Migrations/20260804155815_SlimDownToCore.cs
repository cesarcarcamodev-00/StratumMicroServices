using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SlimDownToCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM inventory.\"InventoryMovements\" WHERE \"ProductId\" IS NULL;");
            migrationBuilder.Sql("DELETE FROM inventory.\"InventoryItems\" WHERE \"ProductId\" IS NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_SubProducts_SubProductId",
                schema: "inventory",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryMovements_SubProducts_SubProductId",
                schema: "inventory",
                table: "InventoryMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                schema: "inventory",
                table: "Products");

            migrationBuilder.DropTable(
                name: "AppSettings",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "DespieceItems",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "Rules",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "Despieces",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "RuleTypes",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "SubProducts",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "Animals",
                schema: "inventory");

            migrationBuilder.DropIndex(
                name: "IX_Products_CategoryId",
                schema: "inventory",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_InventoryMovements_SubProductId",
                schema: "inventory",
                table: "InventoryMovements");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_SubProductId_Location",
                schema: "inventory",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Barcode",
                schema: "inventory",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "inventory",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                schema: "inventory",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubProductId",
                schema: "inventory",
                table: "InventoryMovements");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                schema: "inventory",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "ExpiryDate",
                schema: "inventory",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "MaximumStock",
                schema: "inventory",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "MinimumStock",
                schema: "inventory",
                table: "InventoryItems");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                schema: "inventory",
                table: "Products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "inventory",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                schema: "inventory",
                table: "Products",
                type: "text",
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

            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                schema: "inventory",
                table: "InventoryItems",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDate",
                schema: "inventory",
                table: "InventoryItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaximumStock",
                schema: "inventory",
                table: "InventoryItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinimumStock",
                schema: "inventory",
                table: "InventoryItems",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "SubProductId",
                schema: "inventory",
                table: "InventoryItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Animals",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    DateReceived = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Tag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "inventory",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppSettings",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "String"),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Group = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RuleTypes",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SystemCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    ValueKind = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubProducts",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ReorderLevel = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false, defaultValue: 0m),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "Despieces",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    WasteQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Despieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Despieces_Animals_AnimalId",
                        column: x => x.AnimalId,
                        principalSchema: "inventory",
                        principalTable: "Animals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    RuleTypeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Scope = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "inventory",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rules_RuleTypes_RuleTypeId",
                        column: x => x.RuleTypeId,
                        principalSchema: "inventory",
                        principalTable: "RuleTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rules_SubProducts_SubProductId",
                        column: x => x.SubProductId,
                        principalSchema: "inventory",
                        principalTable: "SubProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DespieceItems",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DespieceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Quantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DespieceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DespieceItems_Despieces_DespieceId",
                        column: x => x.DespieceId,
                        principalSchema: "inventory",
                        principalTable: "Despieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DespieceItems_SubProducts_SubProductId",
                        column: x => x.SubProductId,
                        principalSchema: "inventory",
                        principalTable: "SubProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                schema: "inventory",
                table: "Products",
                column: "CategoryId");

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
                name: "IX_Animals_ProductId_Status",
                schema: "inventory",
                table: "Animals",
                columns: new[] { "ProductId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_Key",
                schema: "inventory",
                table: "AppSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "inventory",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DespieceItems_DespieceId",
                schema: "inventory",
                table: "DespieceItems",
                column: "DespieceId");

            migrationBuilder.CreateIndex(
                name: "IX_DespieceItems_SubProductId",
                schema: "inventory",
                table: "DespieceItems",
                column: "SubProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Despieces_AnimalId",
                schema: "inventory",
                table: "Despieces",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_ProductId",
                schema: "inventory",
                table: "Rules",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_RuleTypeId",
                schema: "inventory",
                table: "Rules",
                column: "RuleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rules_SubProductId",
                schema: "inventory",
                table: "Rules",
                column: "SubProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleTypes_SystemCode",
                schema: "inventory",
                table: "RuleTypes",
                column: "SystemCode",
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
                name: "FK_Products_Categories_CategoryId",
                schema: "inventory",
                table: "Products",
                column: "CategoryId",
                principalSchema: "inventory",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
