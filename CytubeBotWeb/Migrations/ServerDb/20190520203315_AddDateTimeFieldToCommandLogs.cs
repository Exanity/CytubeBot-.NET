using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CytubeBotWeb.Migrations.ServerDb
{
    public partial class AddDateTimeFieldToCommandLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MessageTime",
                table: "CommandLogs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageTime",
                table: "CommandLogs");
        }
    }
}
