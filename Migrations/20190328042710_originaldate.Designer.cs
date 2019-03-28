﻿// <auto-generated />
using System;
using AliasMailApi.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AliasMailApi.Migrations
{
    [DbContext(typeof(MessageContext))]
    [Migration("20190328042710_originaldate")]
    partial class originaldate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.3-servicing-35854")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AliasMailApi.Models.BaseMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("Host")
                        .HasMaxLength(128);

                    b.Property<bool>("Valid");

                    b.Property<DateTime>("Validated");

                    b.HasKey("Id");

                    b.ToTable("Messages");

                    b.HasDiscriminator<string>("Discriminator").HasValue("BaseMessage");
                });

            modelBuilder.Entity("AliasMailApi.Models.MailGunMessage", b =>
                {
                    b.HasBaseType("AliasMailApi.Models.BaseMessage");

                    b.Property<int>("AttachmentCount");

                    b.Property<string>("BodyHtml");

                    b.Property<string>("BodyPlain");

                    b.Property<string>("ContentIdMap");

                    b.Property<string>("ContentType");

                    b.Property<DateTime>("Date");

                    b.Property<string>("From");

                    b.Property<string>("InReplyTo");

                    b.Property<string>("MessageHeaders");

                    b.Property<string>("MessageId");

                    b.Property<string>("MimeVersion");

                    b.Property<string>("OriginalDate");

                    b.Property<string>("OriginalFrom");

                    b.Property<string>("OriginalSender");

                    b.Property<string>("OriginalTo");

                    b.Property<string>("Received");

                    b.Property<string>("Recipient");

                    b.Property<string>("References");

                    b.Property<string>("SFrom");

                    b.Property<string>("SSender");

                    b.Property<string>("SSubject");

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

                    b.HasDiscriminator().HasValue("MailGunMessage");
                });
#pragma warning restore 612, 618
        }
    }
}
