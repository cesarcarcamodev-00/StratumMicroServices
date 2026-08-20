using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryApp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceAnimalsWithCategoryHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Animals_AnimalId",
                schema: "inventory",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Rules_Animals_AnimalId",
                schema: "inventory",
                table: "Rules");

            migrationBuilder.DropTable(
                name: "Animals",
                schema: "inventory");

            migrationBuilder.DropIndex(
                name: "IX_Rules_AnimalId",
                schema: "inventory",
                table: "Rules");

            migrationBuilder.DropIndex(
                name: "IX_Products_AnimalId",
                schema: "inventory",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AnimalId",
                schema: "inventory",
                table: "Rules");

            migrationBuilder.DropColumn(
                name: "AnimalId",
                schema: "inventory",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                schema: "inventory",
                table: "Categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentId",
                schema: "inventory",
                table: "Categories",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentId",
                schema: "inventory",
                table: "Categories",
                column: "ParentId",
                principalSchema: "inventory",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentId",
                schema: "inventory",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentId",
                schema: "inventory",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentId",
                schema: "inventory",
                table: "Categories");

            migrationBuilder.AddColumn<Guid>(
                name: "AnimalId",
                schema: "inventory",
                table: "Rules",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AnimalId",
                schema: "inventory",
                table: "Products",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Animals",
                schema: "inventory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "inventory",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_AnimalId",
                schema: "inventory",
                table: "Rules",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_AnimalId",
                schema: "inventory",
                table: "Products",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_CategoryId",
                schema: "inventory",
                table: "Animals",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Animals_AnimalId",
                schema: "inventory",
                table: "Products",
                column: "AnimalId",
                principalSchema: "inventory",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rules_Animals_AnimalId",
                schema: "inventory",
                table: "Rules",
                column: "AnimalId",
                principalSchema: "inventory",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
