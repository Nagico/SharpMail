using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharpMailBackend.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_account",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    email = table.Column<string>(type: "TEXT", nullable: false),
                    password = table.Column<string>(type: "TEXT", nullable: true),
                    smtp_host = table.Column<string>(type: "TEXT", nullable: true),
                    smtp_port = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 25),
                    smtp_ssl = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    pop3_host = table.Column<string>(type: "TEXT", nullable: true),
                    pop3_ssl = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    pop3_port = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 110),
                    is_active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    create_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    update_time = table.Column<DateTime>(type: "TEXT", nullable: false),
                    last_login_time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_account", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tb_mail",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    account_id = table.Column<int>(type: "INTEGER", nullable: false),
                    uid = table.Column<string>(type: "TEXT", nullable: false),
                    type = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    read = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    subject = table.Column<string>(type: "TEXT", nullable: true),
                    from = table.Column<string>(type: "TEXT", nullable: true),
                    to = table.Column<string>(type: "TEXT", nullable: true),
                    date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    text = table.Column<string>(type: "TEXT", nullable: true),
                    content = table.Column<string>(type: "TEXT", nullable: true),
                    create_time = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_mail", x => x.id);
                    table.ForeignKey(
                        name: "FK_tb_mail_tb_account_account_id",
                        column: x => x.account_id,
                        principalTable: "tb_account",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tb_mail_account_id",
                table: "tb_mail",
                column: "account_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_mail");

            migrationBuilder.DropTable(
                name: "tb_account");
        }
    }
}
