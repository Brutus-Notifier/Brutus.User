using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Brutus.User.Migrations
{
    public partial class ExtendedInvitation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserInvitations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "UserInvitations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserInvitations");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "UserInvitations");
        }
    }
}
