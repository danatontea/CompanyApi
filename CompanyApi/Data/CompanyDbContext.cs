using CompanyApi.Models;
using Microsoft.EntityFrameworkCore;
namespace CompanyApi.Data
{
    public class CompanyDbContext : DbContext
    {
        public CompanyDbContext(DbContextOptions<CompanyDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.StockTicker)
                    .IsRequired()
                    .HasMaxLength(10);
                entity.Property(e => e.Exchange)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Isin)
                    .IsRequired()
                    .HasMaxLength(12);
                entity.Property(e => e.Website)
                    .HasMaxLength(500)
                    .IsRequired(false);
                entity.Property(e => e.CreatedAt)
                    .IsRequired();
                //.HasDefaultValueSql("getutcdate()"); // Use SQL Server's getutcdate() for UTC time  
                entity.Property(e => e.UpdatedAt)
                   .IsRequired()
                    //.HasDefaultValueSql("getutcdate()")
                    .ValueGeneratedOnAddOrUpdate(); // Automatically update on add or update
            });

            // Seed data
            modelBuilder.Entity<Company>().HasData(
                new Company
                {
                    Id = 1,
                    Name = "Apple Inc.",
                    Exchange = "NASDAQ",
                    StockTicker = "AAPL",
                    Isin = "US0378331005",
                    Website = "http://www.apple.com",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = 2,
                    Name = "British Airways Plc",
                    Exchange = "Pink Sheets",
                    StockTicker = "BAIRY",
                    Isin = "US1104193065",
                    Website = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = 3,
                    Name = "Heineken NV",
                    Exchange = "Euronext Amsterdam",
                    StockTicker = "HEIA",
                    Isin = "NL0000009165",
                    Website = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = 4,
                    Name = "Panasonic Corp",
                    Exchange = "Tokyo Stock Exchange",
                    StockTicker = "6752",
                    Isin = "JP3866800000",
                    Website = "http://www.panasonic.co.jp",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Company
                {
                    Id = 5,
                    Name = "Porsche Automobil",
                    Exchange = "Deutsche Börse",
                    StockTicker = "PAH3",
                    Isin = "DE000PAH0038",
                    Website = "https://www.porsche.com/",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

        }
    }
}
