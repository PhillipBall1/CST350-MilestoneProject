using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilestoneProject.Migrations
{
    public partial class UpdateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_registrations",
                table: "registrations");

            migrationBuilder.RenameTable(
                name: "registrations",
                newName: "UserRegistrations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRegistrations",
                table: "UserRegistrations",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRegistrations",
                table: "UserRegistrations");

            migrationBuilder.RenameTable(
                name: "UserRegistrations",
                newName: "registrations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_registrations",
                table: "registrations",
                column: "Id");
        }
    }
}
