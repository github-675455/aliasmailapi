using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AliasMailApi.Migrations
{
    public partial class AbstractBaseModelTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Deleted",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Deleted",
                table: "Mails",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Deleted",
                table: "Mailboxes",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "MailAttachments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Deleted",
                table: "MailAttachments",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Created",
                table: "Domains",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Deleted",
                table: "Domains",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_MailAttachments_Id",
                table: "MailAttachments",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Domains",
                keyColumn: "Id",
                keyValue: new Guid("f49c0b55-451c-4955-a25a-a9a19f8e039f"),
                column: "Created",
                value: new DateTimeOffset(new DateTime(2019, 8, 16, 9, 25, 45, 879, DateTimeKind.Unspecified).AddTicks(1664), new TimeSpan(0, -3, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_MailAttachments_Id",
                table: "MailAttachments");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Mailboxes");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "MailAttachments");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "MailAttachments");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Domains");
        }
    }
}
