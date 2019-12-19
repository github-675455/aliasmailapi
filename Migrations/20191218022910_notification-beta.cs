using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AliasMailApi.Migrations
{
    public partial class notificationbeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(maxLength: 36, nullable: false),
                    Deleted = table.Column<DateTimeOffset>(nullable: true),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(nullable: false),
                    UserAgent = table.Column<string>(maxLength: 1024, nullable: true),
                    RemoteIpAddress = table.Column<string>(maxLength: 45, nullable: true),
                    LastConnected = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevicesSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(maxLength: 36, nullable: false),
                    Deleted = table.Column<DateTimeOffset>(nullable: true),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(nullable: false),
                    DeviceId = table.Column<Guid>(nullable: true),
                    PushEndpoint = table.Column<string>(maxLength: 4096, nullable: true),
                    PushP256DH = table.Column<string>(maxLength: 4096, nullable: true),
                    PushAuth = table.Column<string>(maxLength: 4096, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicesSubscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(maxLength: 36, nullable: false),
                    Deleted = table.Column<DateTimeOffset>(nullable: true),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(nullable: false),
                    Username = table.Column<string>(maxLength: 256, nullable: true),
                    Password = table.Column<string>(maxLength: 512, nullable: true),
                    Salt = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDevice",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    DeviceId = table.Column<Guid>(nullable: false),
                    Id = table.Column<Guid>(maxLength: 36, nullable: false),
                    Deleted = table.Column<DateTimeOffset>(nullable: true),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevice", x => new { x.UserId, x.DeviceId });
                    table.UniqueConstraint("AK_UserDevice_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevice_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDevice_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Domains",
                keyColumn: "Id",
                keyValue: new Guid("f49c0b55-451c-4955-a25a-a9a19f8e039f"),
                column: "Created",
                value: new DateTimeOffset(new DateTime(2019, 12, 17, 23, 29, 10, 125, DateTimeKind.Unspecified).AddTicks(5630), new TimeSpan(0, -3, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_UserDevice_DeviceId",
                table: "UserDevice",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevicesSubscriptions");

            migrationBuilder.DropTable(
                name: "UserDevice");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.UpdateData(
                table: "Domains",
                keyColumn: "Id",
                keyValue: new Guid("f49c0b55-451c-4955-a25a-a9a19f8e039f"),
                column: "Created",
                value: new DateTimeOffset(new DateTime(2019, 11, 5, 13, 48, 45, 296, DateTimeKind.Unspecified).AddTicks(4334), new TimeSpan(0, -3, 0, 0, 0)));
        }
    }
}
