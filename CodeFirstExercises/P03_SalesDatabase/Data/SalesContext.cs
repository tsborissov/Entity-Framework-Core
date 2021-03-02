using Microsoft.EntityFrameworkCore;
using P03_SalesDatabase.Data.Models;

namespace P03_SalesDatabase.Data
{
    public class SalesContext : DbContext
    {
        public SalesContext()
        {
        }

        public SalesContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Store> Stores { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=SalesDatabase;Integrated Security=True");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(x => x.Description)
                .HasMaxLength(250)
                .HasDefaultValue("No description");

            modelBuilder.Entity<Sale>()
                .Property(x => x.Date)
                .HasDefaultValueSql("GETDATE()");
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
