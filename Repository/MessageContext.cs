using System;
using AliasMailApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AliasMailApi.Repository
{
    public class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options) : base(options) { }

        public DbSet<BaseMessage> Messages { get; set; }
        public DbSet<MailgunMessage> MailgunMessages { get; set; }
        public DbSet<Mailbox> Mailboxes { get; set; }
        public DbSet<Domain> Domains { get; set; }
        public DbSet<Mail> Mails { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MailgunMessage>()
            .HasIndex(c => c.Token).IsUnique();
            builder.Entity<Domain>().HasData(new Domain{ Id = Guid.NewGuid(), Name = "vinicius.sl", Description = "", Active = true });
            builder.Entity<Attachment>()
            .HasKey(a => new { a.Name, a.MailId });
        }
    }
}