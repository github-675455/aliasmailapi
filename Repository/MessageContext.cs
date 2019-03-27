using AliasMailApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AliasMailApi.Repository
{
    public class MessageContext : DbContext
    {
        public MessageContext(DbContextOptions<MessageContext> options) : base(options) { }

        public DbSet<BaseMessage> Messages { get; set; }
        public DbSet<MailGunMessage> MailGunMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<MailGunMessage>()
            .HasIndex(c => c.Token).IsUnique();
        }
    }
}