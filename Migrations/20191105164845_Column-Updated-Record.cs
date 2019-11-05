using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AliasMailApi.Migrations
{
    public partial class ColumnUpdatedRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "Messages",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "Mails",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "Mailboxes",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "MailAttachments",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastUpdated",
                table: "Domains",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Domains",
                keyColumn: "Id",
                keyValue: new Guid("f49c0b55-451c-4955-a25a-a9a19f8e039f"),
                column: "Created",
                value: new DateTimeOffset(new DateTime(2019, 11, 5, 13, 48, 45, 296, DateTimeKind.Unspecified).AddTicks(4334), new TimeSpan(0, -3, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Mails");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Mailboxes");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "MailAttachments");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Domains");

            migrationBuilder.UpdateData(
                table: "Domains",
                keyColumn: "Id",
                keyValue: new Guid("f49c0b55-451c-4955-a25a-a9a19f8e039f"),
                column: "Created",
                value: new DateTimeOffset(new DateTime(2019, 8, 16, 9, 25, 45, 879, DateTimeKind.Unspecified).AddTicks(1664), new TimeSpan(0, -3, 0, 0, 0)));
        }
    }
}
