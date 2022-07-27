using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharpMailBackend.Migrations
{
    public partial class AddMailText : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "text",
                table: "tb_mail",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "text",
                table: "tb_mail");
        }
    }
}
