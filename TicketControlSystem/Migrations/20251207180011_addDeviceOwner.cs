using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticket_control_system.Migrations
{
    /// <inheritdoc />
    public partial class addDeviceOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Validations");

            migrationBuilder.AlterColumn<int>(
                name: "Result",
                table: "Validations",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Devices",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Devices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_EventId",
                table: "Devices",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Events_EventId",
                table: "Devices",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Events_EventId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_EventId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Devices");

            migrationBuilder.AlterColumn<string>(
                name: "Result",
                table: "Validations",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Validations",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Devices",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }
    }
}
