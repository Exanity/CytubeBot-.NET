using Microsoft.EntityFrameworkCore.Migrations;

namespace CytubeBotWeb.Migrations.ServerDb
{
    public partial class AddChannelToCommandLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommandLogs_Servers_ChannelModelId",
                table: "CommandLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_CommandLogs_Channels_ChannelModelId",
                table: "CommandLogs",
                column: "ChannelModelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommandLogs_Channels_ChannelModelId",
                table: "CommandLogs");

            migrationBuilder.AddForeignKey(
                name: "FK_CommandLogs_Servers_ChannelModelId",
                table: "CommandLogs",
                column: "ChannelModelId",
                principalTable: "Servers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
