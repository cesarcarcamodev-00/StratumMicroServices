using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveToInventorySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.RenameTable(
                name: "UnitsOfMeasure",
                newName: "UnitsOfMeasure",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Products",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "ProductAttributeValues",
                newName: "ProductAttributeValues",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "InventoryMovements",
                newName: "InventoryMovements",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "InventoryItems",
                newName: "InventoryItems",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "CategoryAttributes",
                newName: "CategoryAttributes",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Categories",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLogs",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "AppSettings",
                newName: "AppSettings",
                newSchema: "inventory");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UnitsOfMeasure",
                schema: "inventory",
                newName: "UnitsOfMeasure");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "inventory",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "ProductAttributeValues",
                schema: "inventory",
                newName: "ProductAttributeValues");

            migrationBuilder.RenameTable(
                name: "InventoryMovements",
                schema: "inventory",
                newName: "InventoryMovements");

            migrationBuilder.RenameTable(
                name: "InventoryItems",
                schema: "inventory",
                newName: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "CategoryAttributes",
                schema: "inventory",
                newName: "CategoryAttributes");

            migrationBuilder.RenameTable(
                name: "Categories",
                schema: "inventory",
                newName: "Categories");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                schema: "inventory",
                newName: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "AppSettings",
                schema: "inventory",
                newName: "AppSettings");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Viewer"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }
    }
}
