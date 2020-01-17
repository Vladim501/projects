using TestWork.Models;
using Microsoft.EntityFrameworkCore;


namespace TestWork
{
    public class LinkDbContext : DbContext
    {
        public LinkDbContext(DbContextOptions<LinkDbContext> options) : base(options)
        {
        }

        public DbSet<Link> Links { get; set; }
        public DbSet<IpAddress> IpAddresses { get; set; }
        public DbSet<IpLink> IpLink { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IpLink>()
                .HasKey(t => new { t.LinkId, t.IpAddressId });

            modelBuilder.Entity<IpLink>()
                .HasOne(pt => pt.Link)
                .WithMany(p => p.IpLinks)
                .HasForeignKey(pt => pt.LinkId);

            modelBuilder.Entity<IpLink>()
                .HasOne(pt => pt.IpAddress)
                .WithMany(t => t.IpLinks)
                .HasForeignKey(pt => pt.IpAddressId);
        }

    }
}
