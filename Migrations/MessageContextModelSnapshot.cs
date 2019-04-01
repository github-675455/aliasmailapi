﻿// <auto-generated />
using System;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AliasMailApi.Migrations
{
    [DbContext(typeof(MessageContext))]
    partial class MessageContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AliasMailApi.Models.Attachment", b =>
                {
                    b.Property<string>("Name");

                    b.Property<Guid>("MailId");

                    b.Property<byte[]>("Data");

                    b.HasKey("Name", "MailId");

                    b.HasIndex("MailId");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("AliasMailApi.Models.BaseMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<bool>("Error");

                    b.Property<string>("ErrorMessage");

                    b.Property<string>("RemoteIpAddress")
                        .HasMaxLength(128);

                    b.Property<bool>("Valid");

                    b.Property<DateTime>("Validated");

                    b.HasKey("Id");

                    b.ToTable("Messages");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseMessage");
                });

            modelBuilder.Entity("AliasMailApi.Models.Domain", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Description")
                        .HasMaxLength(2048);

                    b.Property<string>("Name")
                        .HasMaxLength(253);

                    b.HasKey("Id");

                    b.ToTable("Domains");

                    b.HasData(
                        new
                        {
                            Id = new Guid("1b349c6a-7220-428d-82fc-f56a04d3bf8e"),
                            Active = true,
                            Description = "",
                            Name = "vinicius.sl"
                        });
                });

            modelBuilder.Entity("AliasMailApi.Models.Mail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Attachments");

                    b.Property<Guid?>("BaseMessageId");

                    b.Property<string>("BodyHtml");

                    b.Property<string>("BodyPlain");

                    b.Property<DateTimeOffset>("Created");

                    b.Property<DateTimeOffset>("Date");

                    b.Property<string>("FromAddress")
                        .HasMaxLength(254);

                    b.Property<string>("FromDisplayName")
                        .HasMaxLength(254);

                    b.Property<string>("InReplyTo");

                    b.Property<string>("MessageId");

                    b.Property<string>("OriginalDate");

                    b.Property<string>("Received");

                    b.Property<string>("Recipient");

                    b.Property<string>("References");

                    b.Property<string>("SenderAddress")
                        .HasMaxLength(254);

                    b.Property<string>("SenderDisplayName")
                        .HasMaxLength(254);

                    b.Property<string>("Subject");

                    b.Property<string>("ToAddress")
                        .HasMaxLength(254);

                    b.Property<string>("ToDisplayName")
                        .HasMaxLength(254);

                    b.Property<string>("UserAgent")
                        .HasMaxLength(4096);

                    b.Property<string>("remoteIpAddress");

                    b.HasKey("Id");

                    b.HasIndex("BaseMessageId");

                    b.ToTable("Mails");
                });

            modelBuilder.Entity("AliasMailApi.Models.Mailbox", b =>
                {
                    b.Property<string>("Email")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(512);

                    b.Property<DateTime>("Created");

                    b.Property<bool>("CreatedManually");

                    b.Property<bool>("Delete");

                    b.Property<string>("Description")
                        .HasMaxLength(2048);

                    b.Property<Guid>("DomainId");

                    b.Property<bool>("Reject");

                    b.Property<int>("StoreQuantity");

                    b.HasKey("Email");

                    b.HasIndex("DomainId");

                    b.ToTable("Mailboxes");
                });

            modelBuilder.Entity("AliasMailApi.Models.MailgunMessage", b =>
                {
                    b.HasBaseType("AliasMailApi.Models.BaseMessage");

                    b.Property<int>("AttachmentCount");

                    b.Property<string>("Attachments");

                    b.Property<string>("BodyHtml");

                    b.Property<string>("BodyPlain");

                    b.Property<string>("ContentIdMap");

                    b.Property<string>("ContentType");

                    b.Property<string>("Date");

                    b.Property<string>("From");

                    b.Property<string>("InReplyTo");

                    b.Property<string>("MessageHeaders");

                    b.Property<string>("MessageId");

                    b.Property<string>("MimeVersion");

                    b.Property<string>("Received");

                    b.Property<string>("Recipient");

                    b.Property<string>("References");

                    b.Property<string>("Sender");

                    b.Property<string>("Signature");

                    b.Property<string>("StrippedHtml");

                    b.Property<string>("StrippedSignature");

                    b.Property<string>("StrippedText");

                    b.Property<string>("Subject");

                    b.Property<string>("Timestamp");

                    b.Property<string>("To");

                    b.Property<string>("Token")
                        .HasMaxLength(50);

                    b.Property<string>("UserAgent");

                    b.Property<string>("XMailgunVariables");

                    b.HasIndex("Token")
                        .IsUnique();

                    b.HasDiscriminator().HasValue("MailgunMessage");
                });

            modelBuilder.Entity("AliasMailApi.Models.Attachment", b =>
                {
                    b.HasOne("AliasMailApi.Models.Mail", "mail")
                        .WithMany()
                        .HasForeignKey("MailId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AliasMailApi.Models.Mail", b =>
                {
                    b.HasOne("AliasMailApi.Models.BaseMessage", "BaseMessage")
                        .WithMany()
                        .HasForeignKey("BaseMessageId");
                });

            modelBuilder.Entity("AliasMailApi.Models.Mailbox", b =>
                {
                    b.HasOne("AliasMailApi.Models.Domain", "Domain")
                        .WithMany()
                        .HasForeignKey("DomainId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
