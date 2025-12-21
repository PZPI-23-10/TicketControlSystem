using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticket_control_system.Migrations
{
    /// <inheritdoc />
    public partial class newfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastHeartbeat",
                table: "Devices",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastHeartbeat",
                table: "Devices");
        }
    }
}
