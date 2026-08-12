using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support_System_API.Migrations
{
    /// <inheritdoc />
    public partial class NewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TicketHistories");

            migrationBuilder.AddColumn<string>(
                name: "NewValue",
                table: "TicketHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OldValue",
                table: "TicketHistories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewValue",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "OldValue",
                table: "TicketHistories");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TicketHistories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
