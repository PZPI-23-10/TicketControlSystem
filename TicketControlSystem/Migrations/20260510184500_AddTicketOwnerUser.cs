using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Ticket_control_system.Data;

#nullable disable

namespace Ticket_control_system.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260510184500_AddTicketOwnerUser")]
    public partial class AddTicketOwnerUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerUserId",
                table: "Tickets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OwnerUserId",
                table: "Tickets",
                column: "OwnerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerUserId",
                table: "Tickets",
                column: "OwnerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_AspNetUsers_OwnerUserId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_OwnerUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Tickets");
        }
    }
}
