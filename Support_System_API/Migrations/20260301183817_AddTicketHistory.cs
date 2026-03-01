using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support_System_API.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "TicketHistories");

            migrationBuilder.RenameColumn(
                name: "OldStatus",
                table: "TicketHistories",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ChangedAt",
                table: "TicketHistories",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TicketHistories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TicketHistories");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "TicketHistories",
                newName: "OldStatus");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "TicketHistories",
                newName: "ChangedAt");

            migrationBuilder.AddColumn<int>(
                name: "NewStatus",
                table: "TicketHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
