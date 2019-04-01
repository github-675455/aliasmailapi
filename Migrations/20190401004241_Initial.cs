using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AliasMailApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 253, nullable: true),
                    Description = table.Column<string>(maxLength: 2048, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Valid = table.Column<bool>(nullable: false),
                    Validated = table.Column<DateTime>(nullable: false),
                    Error = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(nullable: true),
                    RemoteIpAddress = table.Column<string>(maxLength: 128, nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    From = table.Column<string>(nullable: true),
                    InReplyTo = table.Column<string>(nullable: true),
                    MessageId = table.Column<string>(nullable: true),
                    MimeVersion = table.Column<string>(nullable: true),
                    Received = table.Column<string>(nullable: true),
                    References = table.Column<string>(nullable: true),
                    Sender = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    XMailgunVariables = table.Column<string>(nullable: true),
                    Attachments = table.Column<string>(nullable: true),
                    AttachmentCount = table.Column<int>(nullable: true),
                    BodyHtml = table.Column<string>(nullable: true),
                    BodyPlain = table.Column<string>(nullable: true),
                    ContentIdMap = table.Column<string>(nullable: true),
                    MessageHeaders = table.Column<string>(nullable: true),
                    Recipient = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true),
                    StrippedHtml = table.Column<string>(nullable: true),
                    StrippedSignature = table.Column<string>(nullable: true),
                    StrippedText = table.Column<string>(nullable: true),
                    Timestamp = table.Column<string>(nullable: true),
                    Token = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mailboxes",
                columns: table => new
                {
                    Email = table.Column<string>(maxLength: 512, nullable: false),
                    DomainId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(maxLength: 2048, nullable: true),
                    StoreQuantity = table.Column<int>(nullable: false),
                    Reject = table.Column<bool>(nullable: false),
                    Delete = table.Column<bool>(nullable: false),
                    CreatedManually = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mailboxes", x => x.Email);
                    table.ForeignKey(
                        name: "FK_Mailboxes_Domains_DomainId",
                        column: x => x.DomainId,
                        principalTable: "Domains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    SenderAddress = table.Column<string>(maxLength: 254, nullable: true),
                    SenderDisplayName = table.Column<string>(maxLength: 254, nullable: true),
                    FromAddress = table.Column<string>(maxLength: 254, nullable: true),
                    FromDisplayName = table.Column<string>(maxLength: 254, nullable: true),
                    ToAddress = table.Column<string>(maxLength: 254, nullable: true),
                    ToDisplayName = table.Column<string>(maxLength: 254, nullable: true),
                    Date = table.Column<DateTimeOffset>(nullable: false),
                    OriginalDate = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(maxLength: 4096, nullable: true),
                    InReplyTo = table.Column<string>(nullable: true),
                    MessageId = table.Column<string>(nullable: true),
                    Received = table.Column<string>(nullable: true),
                    References = table.Column<string>(nullable: true),
                    Attachments = table.Column<string>(nullable: true),
                    BodyHtml = table.Column<string>(nullable: true),
                    BodyPlain = table.Column<string>(nullable: true),
                    Recipient = table.Column<string>(nullable: true),
                    remoteIpAddress = table.Column<string>(nullable: true),
                    BaseMessageId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mails_Messages_BaseMessageId",
                        column: x => x.BaseMessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    MailId = table.Column<Guid>(nullable: false),
                    Data = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => new { x.Name, x.MailId });
                    table.ForeignKey(
                        name: "FK_Attachment_Mails_MailId",
                        column: x => x.MailId,
                        principalTable: "Mails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Domains",
                columns: new[] { "Id", "Active", "Description", "Name" },
                values: new object[] { new Guid("1b349c6a-7220-428d-82fc-f56a04d3bf8e"), true, "", "vinicius.sl" });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_MailId",
                table: "Attachment",
                column: "MailId");

            migrationBuilder.CreateIndex(
                name: "IX_Mailboxes_DomainId",
                table: "Mailboxes",
                column: "DomainId");

            migrationBuilder.CreateIndex(
                name: "IX_Mails_BaseMessageId",
                table: "Mails",
                column: "BaseMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_Token",
                table: "Messages",
                column: "Token",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "Mailboxes");

            migrationBuilder.DropTable(
                name: "Mails");

            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}
