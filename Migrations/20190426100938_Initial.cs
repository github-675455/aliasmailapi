﻿using System;
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
                    Id = table.Column<Guid>(maxLength: 36, nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    Valid = table.Column<bool>(nullable: false),
                    Validated = table.Column<DateTimeOffset>(nullable: false),
                    Error = table.Column<bool>(nullable: false),
                    ErrorMessage = table.Column<string>(maxLength: 4096, nullable: true),
                    RemoteIpAddress = table.Column<string>(maxLength: 45, nullable: true),
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
                    Id = table.Column<Guid>(maxLength: 36, nullable: false),
                    Created = table.Column<DateTimeOffset>(nullable: false),
                    SenderAddress = table.Column<string>(maxLength: 256, nullable: true),
                    SenderDisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    FromAddress = table.Column<string>(maxLength: 256, nullable: true),
                    FromDisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    ToAddress = table.Column<string>(maxLength: 256, nullable: true),
                    ToDisplayName = table.Column<string>(maxLength: 256, nullable: true),
                    Date = table.Column<DateTimeOffset>(nullable: true),
                    OriginalDate = table.Column<string>(maxLength: 256, nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(maxLength: 4096, nullable: true),
                    InReplyTo = table.Column<string>(maxLength: 4096, nullable: true),
                    MessageId = table.Column<string>(maxLength: 4096, nullable: true),
                    Received = table.Column<string>(maxLength: 16384, nullable: true),
                    References = table.Column<string>(maxLength: 4096, nullable: true),
                    Attachments = table.Column<string>(maxLength: 16384, nullable: true),
                    MailAttachmentsJobStatus = table.Column<string>(unicode: false, maxLength: 32, nullable: false),
                    MailAttachmentsJobErrorMessage = table.Column<string>(maxLength: 4096, nullable: true),
                    BodyHtml = table.Column<string>(maxLength: 10485760, nullable: true),
                    BodyPlain = table.Column<string>(maxLength: 10485760, nullable: true),
                    Recipient = table.Column<string>(nullable: true),
                    remoteIpAddress = table.Column<string>(maxLength: 45, nullable: true),
                    BaseMessageId = table.Column<Guid>(maxLength: 36, nullable: true),
                    JobStats = table.Column<int>(nullable: false),
                    ErrorMessage = table.Column<string>(maxLength: 4096, nullable: true),
                    ErrorDate = table.Column<DateTimeOffset>(nullable: true),
                    Retries = table.Column<int>(nullable: false),
                    NextRetry = table.Column<DateTimeOffset>(nullable: true),
                    Source = table.Column<int>(nullable: false)
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
                name: "MailAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    MailId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Data = table.Column<byte[]>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailAttachments", x => new { x.Id, x.MailId });
                    table.ForeignKey(
                        name: "FK_MailAttachments_Mails_MailId",
                        column: x => x.MailId,
                        principalTable: "Mails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Domains",
                columns: new[] { "Id", "Active", "Description", "Name" },
                values: new object[] { new Guid("f49c0b55-451c-4955-a25a-a9a19f8e039f"), true, "", "vinicius.sl" });

            migrationBuilder.CreateIndex(
                name: "IX_MailAttachments_MailId",
                table: "MailAttachments",
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
                name: "MailAttachments");

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
