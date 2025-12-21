using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticket_control_system.Migrations
{
    /// <inheritdoc />
    public partial class addAttr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Validations_Users_UserId",
                table: "Validations");

            migrationBuilder.DropIndex(
                name: "IX_Validations_UserId",
                table: "Validations");

            migrationBuilder.RenameColumn(
                name: "DeviceUid",
                table: "Devices",
                newName: "Location");

            migrationBuilder.AddColumn<int>(
                name: "CurrentUses",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxUses",
                table: "Tickets",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "EventType",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Events_OwnerId",
                table: "Events",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Users_OwnerId",
                table: "Events",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Users_OwnerId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_OwnerId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "CurrentUses",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "MaxUses",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Devices",
                newName: "DeviceUid");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "Events",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Validations_UserId",
                table: "Validations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Validations_Users_UserId",
                table: "Validations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
