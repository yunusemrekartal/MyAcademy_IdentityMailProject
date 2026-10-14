using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Context
{
    public class AppDbContext:IdentityDbContext<AppUser,AppRole,int>
    {
        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>().HasMany(message => message.SentMessages).WithOne(s => s.Sender).HasForeignKey(x => x.SenderId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AppUser>().HasMany(message => message.ReceivedMessages).WithOne(s => s.Receiver).HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserMessage>()
            .HasOne(m => m.Category)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<MessageCategory> MessageCategories { get; set; }
    }
}
