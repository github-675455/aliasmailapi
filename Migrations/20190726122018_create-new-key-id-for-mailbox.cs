using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AliasMailApi.Migrations
{
    public partial class createnewkeyidformailbox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Mailboxes",
                table: "Mailboxes");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Mailboxes",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Mailboxes",
                nullable: false,
                defaultValue: new Guid());

            migrationBuilder.Sql("UPDATE Mailboxes SET Id = UUID();");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mailboxes",
                table: "Mailboxes",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Mailboxes",
                table: "Mailboxes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Mailboxes");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Mailboxes",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mailboxes",
                table: "Mailboxes",
                column: "Email");
        }
    }
}
