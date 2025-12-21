using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticket_control_system.Migrations
{
    /// <inheritdoc />
    public partial class addTicketOwnerName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TicketOwnerName",
                table: "Tickets",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketOwnerName",
                table: "Tickets");
        }
    }
}
