using Microsoft.EntityFrameworkCore;
using TestXML.Models;

namespace TestXML.Context
{
    public class MarketDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=testXml;Trusted_Connection=True;Integrated Security=True;TrustServerCertificate=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Products);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Order)
                .WithMany()
                .HasForeignKey(po => po.OrderId);

            modelBuilder.Entity<ProductOrder>()
                .HasOne(po => po.Product);
        }
    }
}
