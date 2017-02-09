using System.Data.Entity;
using Absentia.Model.Entities;

namespace Absentia.Model
{
    public class AbsentiaDbContext : DbContext
    {
        public DbSet<DirectoryUser> Users { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<SubscriptionAttempt> SubscriptionAttempts { get; set; }

        public AbsentiaDbContext()
        {
            Database.SetInitializer<AbsentiaDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DirectoryUser>()
                .HasMany(x => x.Subscriptions)
                .WithRequired(x => x.User);

            modelBuilder.Entity<SubscriptionAttempt>()
                .HasKey(x => x.SubscriptionAttemptId);

            modelBuilder.Entity<Subscription>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Subscription>()
                .Property(x => x.Id)
                .HasMaxLength(200);

            modelBuilder.Entity<Notification>()
                .HasOptional(x => x.ResourceData)
                .WithRequired()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ResourceData>()
                .HasKey(x => x.ResourceDataId);

        }
    }
}