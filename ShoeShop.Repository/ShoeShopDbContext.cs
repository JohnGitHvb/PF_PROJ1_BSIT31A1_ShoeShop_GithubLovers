using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository.Entities;

namespace ShoeShop.Repository
{
    public class ShoeShopDbContext : DbContext
    {
        public ShoeShopDbContext(DbContextOptions<ShoeShopDbContext> options)
            : base(options)
        {
        }

        // DbSets - represent tables in the database
        public DbSet<Shoe> Shoes { get; set; }
        public DbSet<ShoeColorVariation> ShoeColorVariations { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<StockPullOut> StockPullOuts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Shoe to ShoeColorVariations (One-to-Many)
            modelBuilder.Entity<Shoe>()
                .HasMany(s => s.ColorVariations)
                .WithOne(cv => cv.Shoe)
                .HasForeignKey(cv => cv.ShoeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Supplier to PurchaseOrders (One-to-Many)
            modelBuilder.Entity<Supplier>()
                .HasMany(s => s.PurchaseOrders)
                .WithOne(po => po.Supplier)
                .HasForeignKey(po => po.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchaseOrder to PurchaseOrderItems (One-to-Many)
            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(po => po.OrderItems)
                .WithOne(poi => poi.PurchaseOrder)
                .HasForeignKey(poi => poi.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ShoeColorVariation to PurchaseOrderItems (One-to-Many)
            modelBuilder.Entity<ShoeColorVariation>()
                .HasMany(scv => scv.PurchaseOrderItems)
                .WithOne(poi => poi.ShoeColorVariation)
                .HasForeignKey(poi => poi.ShoeColorVariationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ShoeColorVariation to StockPullOuts (One-to-Many)
            modelBuilder.Entity<ShoeColorVariation>()
                .HasMany(scv => scv.StockPullOuts)
                .WithOne(spo => spo.ShoeColorVariation)
                .HasForeignKey(spo => spo.ShoeColorVariationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Suppliers
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Nike Wholesale", ContactEmail = "orders@nikewholesale.com", ContactPhone = "555-0101", Address = "123 Nike Street, Portland, OR", IsActive = true },
                new Supplier { Id = 2, Name = "Adidas Distribution", ContactEmail = "sales@adidascorp.com", ContactPhone = "555-0102", Address = "456 Adidas Ave, Portland, OR", IsActive = true },
                new Supplier { Id = 3, Name = "Puma International", ContactEmail = "info@pumaint.com", ContactPhone = "555-0103", Address = "789 Puma Blvd, Portland, OR", IsActive = true }
            );

            // Seed Shoes - FIXED: Using static date
            modelBuilder.Entity<Shoe>().HasData(
                new Shoe { Id = 1, Name = "Air Max 270", Brand = "Nike", Cost = 85.00m, Price = 150.00m, Description = "Comfortable running shoes with air cushioning", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 2, Name = "Ultra Boost 22", Brand = "Adidas", Cost = 95.00m, Price = 180.00m, Description = "Energy-returning running shoes", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 3, Name = "Suede Classic", Brand = "Puma", Cost = 45.00m, Price = 80.00m, Description = "Iconic suede sneakers", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 4, Name = "Air Jordan 1", Brand = "Nike", Cost = 120.00m, Price = 200.00m, Description = "Classic basketball shoes", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 5, Name = "Superstar", Brand = "Adidas", Cost = 60.00m, Price = 100.00m, Description = "Classic shell-toe design", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 6, Name = "RS-X", Brand = "Puma", Cost = 70.00m, Price = 120.00m, Description = "Bold and chunky sneakers", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 7, Name = "Cortez", Brand = "Nike", Cost = 50.00m, Price = 90.00m, Description = "Retro running shoes", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 8, Name = "Stan Smith", Brand = "Adidas", Cost = 55.00m, Price = 95.00m, Description = "Minimalist tennis shoes", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 9, Name = "Clyde Court", Brand = "Puma", Cost = 80.00m, Price = 140.00m, Description = "Performance basketball shoes", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 10, Name = "Blazer Mid", Brand = "Nike", Cost = 65.00m, Price = 110.00m, Description = "Classic high-top sneakers", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 11, Name = "NMD R1", Brand = "Adidas", Cost = 90.00m, Price = 160.00m, Description = "Modern nomad sneakers", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 12, Name = "Future Rider", Brand = "Puma", Cost = 55.00m, Price = 95.00m, Description = "Retro-futuristic running shoes", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 13, Name = "Dunk Low", Brand = "Nike", Cost = 75.00m, Price = 130.00m, Description = "Versatile skateboarding shoes", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 14, Name = "Yeezy Boost 350", Brand = "Adidas", Cost = 150.00m, Price = 250.00m, Description = "Premium lifestyle sneakers", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) },
                new Shoe { Id = 15, Name = "Speedcat", Brand = "Puma", Cost = 60.00m, Price = 105.00m, Description = "Motorsport-inspired shoes", IsActive = true, CreatedDate = new DateTime(2025, 1, 1) }
            );

            // Seed Color Variations with Stock (no changes needed here)
            modelBuilder.Entity<ShoeColorVariation>().HasData(
                // Air Max 270
                new ShoeColorVariation { Id = 1, ShoeId = 1, ColorName = "Black", HexCode = "#000000", StockQuantity = 25, ReorderLevel = 5, IsActive = true },
                new ShoeColorVariation { Id = 2, ShoeId = 1, ColorName = "White", HexCode = "#FFFFFF", StockQuantity = 30, ReorderLevel = 5, IsActive = true },
                new ShoeColorVariation { Id = 3, ShoeId = 1, ColorName = "Red", HexCode = "#FF0000", StockQuantity = 15, ReorderLevel = 5, IsActive = true },
                // Ultra Boost 22
                new ShoeColorVariation { Id = 4, ShoeId = 2, ColorName = "Blue", HexCode = "#0000FF", StockQuantity = 20, ReorderLevel = 5, IsActive = true },
                new ShoeColorVariation { Id = 5, ShoeId = 2, ColorName = "Grey", HexCode = "#808080", StockQuantity = 18, ReorderLevel = 5, IsActive = true },
                // Suede Classic
                new ShoeColorVariation { Id = 6, ShoeId = 3, ColorName = "Navy", HexCode = "#000080", StockQuantity = 12, ReorderLevel = 5, IsActive = true },
                new ShoeColorVariation { Id = 7, ShoeId = 3, ColorName = "Red", HexCode = "#FF0000", StockQuantity = 10, ReorderLevel = 5, IsActive = true },
                // Air Jordan 1
                new ShoeColorVariation { Id = 8, ShoeId = 4, ColorName = "Chicago Red", HexCode = "#CD5C5C", StockQuantity = 8, ReorderLevel = 5, IsActive = true },
                new ShoeColorVariation { Id = 9, ShoeId = 4, ColorName = "Black", HexCode = "#000000", StockQuantity = 3, ReorderLevel = 5, IsActive = true },
                // Superstar
                new ShoeColorVariation { Id = 10, ShoeId = 5, ColorName = "White", HexCode = "#FFFFFF", StockQuantity = 35, ReorderLevel = 5, IsActive = true },
                new ShoeColorVariation { Id = 11, ShoeId = 5, ColorName = "Black", HexCode = "#000000", StockQuantity = 22, ReorderLevel = 5, IsActive = true },
                // RS-X
                new ShoeColorVariation { Id = 12, ShoeId = 6, ColorName = "Multi", HexCode = "#FF6347", StockQuantity = 14, ReorderLevel = 5, IsActive = true },
                // Cortez
                new ShoeColorVariation { Id = 13, ShoeId = 7, ColorName = "White/Red", HexCode = "#FFFFFF", StockQuantity = 16, ReorderLevel = 5, IsActive = true },
                // Stan Smith
                new ShoeColorVariation { Id = 14, ShoeId = 8, ColorName = "White/Green", HexCode = "#FFFFFF", StockQuantity = 28, ReorderLevel = 5, IsActive = true },
                // Clyde Court
                new ShoeColorVariation { Id = 15, ShoeId = 9, ColorName = "Black", HexCode = "#000000", StockQuantity = 10, ReorderLevel = 5, IsActive = true },
                // Blazer Mid
                new ShoeColorVariation { Id = 16, ShoeId = 10, ColorName = "White", HexCode = "#FFFFFF", StockQuantity = 20, ReorderLevel = 5, IsActive = true },
                // NMD R1
                new ShoeColorVariation { Id = 17, ShoeId = 11, ColorName = "Black/Blue", HexCode = "#000000", StockQuantity = 15, ReorderLevel = 5, IsActive = true },
                // Future Rider
                new ShoeColorVariation { Id = 18, ShoeId = 12, ColorName = "Grey/Yellow", HexCode = "#808080", StockQuantity = 18, ReorderLevel = 5, IsActive = true },
                // Dunk Low
                new ShoeColorVariation { Id = 19, ShoeId = 13, ColorName = "Panda", HexCode = "#000000", StockQuantity = 5, ReorderLevel = 5, IsActive = true },
                // Yeezy Boost 350
                new ShoeColorVariation { Id = 20, ShoeId = 14, ColorName = "Cream", HexCode = "#FFFDD0", StockQuantity = 4, ReorderLevel = 5, IsActive = true },
                // Speedcat
                new ShoeColorVariation { Id = 21, ShoeId = 15, ColorName = "Black/Red", HexCode = "#000000", StockQuantity = 12, ReorderLevel = 5, IsActive = true }
            );
        }
    }
}