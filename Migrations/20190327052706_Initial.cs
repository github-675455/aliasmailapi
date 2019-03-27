using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AliasMailApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Valid = table.Column<bool>(nullable: false),
                    Validated = table.Column<DateTime>(nullable: false),
                    Host = table.Column<string>(maxLength: 128, nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true),
                    OriginalFrom = table.Column<string>(nullable: true),
                    From = table.Column<string>(nullable: true),
                    InReplyTo = table.Column<string>(nullable: true),
                    MessageId = table.Column<string>(nullable: true),
                    MimeVersion = table.Column<string>(nullable: true),
                    Received = table.Column<string>(nullable: true),
                    References = table.Column<string>(nullable: true),
                    OriginalSender = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    OriginalTo = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    XMailgunVariables = table.Column<string>(nullable: true),
                    AttachmentCount = table.Column<int>(nullable: true),
                    BodyHtml = table.Column<string>(nullable: true),
                    BodyPlain = table.Column<string>(nullable: true),
                    ContentIdMap = table.Column<string>(nullable: true),
                    SFrom = table.Column<string>(nullable: true),
                    MessageHeaders = table.Column<string>(nullable: true),
                    Recipient = table.Column<string>(nullable: true),
                    SSender = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    StrippedHtml = table.Column<string>(nullable: true),
                    StrippedSignature = table.Column<string>(nullable: true),
                    StrippedText = table.Column<string>(nullable: true),
                    SSubject = table.Column<string>(nullable: true),
                    Timestamp = table.Column<string>(nullable: true),
                    Token = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Token",
                table: "Messages",
                column: "Token",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
