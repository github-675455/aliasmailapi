using System;
using AliasMailApi.Models;
using AliasMailApi.Models.Enum;
using Microsoft.EntityFrameworkCore;

namespace AliasMailApi.Repository
{
    public class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options) : base(options) {}

        public DbSet<BaseMessage> Messages { get; set; }
        public DbSet<MailgunMessage> MailgunMessages { get; set; }
        public DbSet<Mailbox> Mailboxes { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Mail> Mails { get; set; }
        public DbSet<MailAttachment> MailAttachments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MailgunMessage>()
            .HasIndex(c => c.Token).IsUnique();

            builder.Entity<Domain>()
            .HasData(new Domain{ Id = new Guid("f49c0b55-451c-4955-a25a-a9a19f8e039f"), Name = "vinicius.sl", Description = "", Active = true });

            builder.Entity<MailAttachment>()
            .HasKey(a => new { a.Id, a.MailId });
            
            builder.Entity<Mail>()
            .Property(s => s.MailAttachmentsJobStatus)
            .HasConversion(
                v => v.ToString(),
                v => (JobStats)Enum.Parse(typeof(JobStats),v))
                .IsUnicode(false);
        }
    }
}