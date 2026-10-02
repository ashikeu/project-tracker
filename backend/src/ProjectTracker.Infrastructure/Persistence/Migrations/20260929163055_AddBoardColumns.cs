using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BoardColumnId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BoardColumns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    WipLimit = table.Column<int>(type: "int", nullable: true),
                    StatusMapping = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsCompletionColumn = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardColumns_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BoardColumnId",
                table: "Tasks",
                column: "BoardColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardColumns_ProjectId_Order",
                table: "BoardColumns",
                columns: new[] { "ProjectId", "Order" });

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_BoardColumns_BoardColumnId",
                table: "Tasks",
                column: "BoardColumnId",
                principalTable: "BoardColumns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_BoardColumns_BoardColumnId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "BoardColumns");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_BoardColumnId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "BoardColumnId",
                table: "Tasks");
        }
    }
}
