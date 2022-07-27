using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharpMailBackend.Migrations
{
    public partial class AddMailRead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "read",
                table: "tb_mail",
                type: "INTEGER",
                nullable: true,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "read",
                table: "tb_mail");
        }
    }
}
