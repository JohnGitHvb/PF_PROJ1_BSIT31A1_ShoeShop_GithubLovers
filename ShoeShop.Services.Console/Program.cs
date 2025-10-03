using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
// Use your repository/DbContext namespace:
using ShoeShop.Data;   // <-- you already used this in your snippet
using ShoeShop.Entities;
using ShoeShop.Services.DTOs.PullOuts;
using ShoeShop.Services.DTOs.PurchaseOrders;
using ShoeShop.Services.DTOs.Reports;
using ShoeShop.Services.DTOs.Shoes;
using ShoeShop.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var services = new ServiceCollection();

        services.AddDbContext<ShoeShopDbContext>(opts =>
            opts.UseSqlite("Data Source=shoe_shop_demo.db"));

        // Register Service layer implementations
        services.AddScoped<IInventoryService, ShoeShop.Services.Services.InventoryService>();
        services.AddScoped<IPurchaseOrderService, ShoeShop.Services.Services.PurchaseOrderService>();
        services.AddScoped<IPullOutService, ShoeShop.Services.Services.PullOutService>();
        services.AddScoped<IReportService, ShoeShop.Services.Services.ReportService>();

        var sp = services.BuildServiceProvider();

        using (var scope = sp.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ShoeShopDbContext>();
            db.Database.EnsureCreated();
        }

        await RunDemo(sp);
    }

    static async Task RunDemo(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ShoeShopDbContext>();
        var inventory = scope.ServiceProvider.GetRequiredService<IInventoryService>();
        var poService = scope.ServiceProvider.GetRequiredService<IPurchaseOrderService>();
        var pullService = scope.ServiceProvider.GetRequiredService<IPullOutService>();
        var report = scope.ServiceProvider.GetRequiredService<IReportService>();

        Console.WriteLine("=== ShoeShop Services Demo ===\n");

        try
        {
            var supplier = db.Suppliers.FirstOrDefault();
            if (supplier == null)
            {
                supplier = new Supplier
                {
                    Name = "Demo Supplier",
                    ContactEmail = "supplier@example.com",
                    ContactPhone = "09171234567",
                    Address = "Demo address",
                    IsActive = true
                };
                db.Suppliers.Add(supplier);
                await db.SaveChangesAsync();
                Console.WriteLine($"Seeded Supplier: {supplier.Id} - {supplier.Name}");
            }
            else
            {
                Console.WriteLine($"Using existing Supplier: {supplier.Id} - {supplier.Name}");
            }

            var createShoeDto = new CreateShoeDto
            {
                Name = "Demo Runner",
                Brand = "DemoBrand",
                Cost = 20m,
                Price = 40m,
                Description = "Demo shoe for end-to-end test",
                ImageUrl = null,
                IsActive = true
            };

            var shoeDto = await inventory.CreateShoeAsync(createShoeDto);
            Console.WriteLine($"\nCreated Shoe: {shoeDto.Id} - {shoeDto.Name} (Brand: {shoeDto.Brand})");

            var variation = db.ShoeColorVariations.FirstOrDefault(v => v.ShoeId == shoeDto.Id);
            if (variation == null)
            {
                variation = new ShoeColorVariation
                {
                    ShoeId = shoeDto.Id,
                    ColorName = "Black",
                    HexCode = "#000000",
                    StockQuantity = 0,
                    ReorderLevel = 5,
                    IsActive = true
                };
                db.ShoeColorVariations.Add(variation);
                await db.SaveChangesAsync();
                Console.WriteLine($"Created ShoeColorVariation: {variation.Id} - {variation.ColorName}");
            }
            else
            {
                Console.WriteLine($"Using existing variation: {variation.Id} - {variation.ColorName} (Stock {variation.StockQuantity})");
            }

            var poNumber = $"PO-{DateTime.UtcNow:yyyyMMddHHmmss}";
            var createPoDto = new CreatePurchaseOrderDto
            {
                OrderNumber = poNumber,
                SupplierId = supplier.Id,
                OrderDate = DateTime.UtcNow,
                ExpectedDate = DateTime.UtcNow.AddDays(7),
                Items = {
                    new CreatePurchaseOrderItemDto
                    {
                        ShoeColorVariationId = variation.Id,
                        QuantityOrdered = 20,
                        UnitCost = 18m
                    }
                }
            };

            var createdPo = await poService.CreatePurchaseOrderAsync(createPoDto);
            Console.WriteLine($"\nCreated Purchase Order: {createdPo.Id} ({createdPo.OrderNumber}) - Total {createdPo.TotalAmount:C}");

            await poService.ConfirmPurchaseOrderAsync(createdPo.Id);
            var poAfterConfirm = await poService.GetPurchaseOrderAsync(createdPo.Id);
            Console.WriteLine($"PO Status after confirm: {poAfterConfirm.Status}");

            var firstItem = poAfterConfirm.Items.First();
            Console.WriteLine($"\nReceiving {firstItem.QuantityOrdered} units for PO Item {firstItem.Id}...");
            await poService.ReceivePurchaseOrderItemAsync(firstItem.Id, firstItem.QuantityOrdered);

            var poAfterReceive = await poService.GetPurchaseOrderAsync(createdPo.Id);
            Console.WriteLine($"PO Status after receive: {poAfterReceive.Status}");
            Console.WriteLine($"PO Item received qty: {poAfterReceive.Items.First().QuantityReceived}");

            var updatedVar = await db.ShoeColorVariations.FindAsync(variation.Id);
            Console.WriteLine($"Variation stock after receive: {updatedVar.StockQuantity}");

            var createPullDto = new CreatePullOutDto
            {
                ShoeColorVariationId = variation.Id,
                Quantity = 3,
                Reason = "Promotional giveaway",
                ReasonDetails = "Giveaway for marketing",
                RequestedBy = "StaffA"
            };

            var pullReq = await pullService.CreatePullOutRequestAsync(createPullDto);
            Console.WriteLine($"\nCreated Pull-Out request: ID {pullReq.Id}, Qty {pullReq.Quantity}, Status {pullReq.Status}");

            var approved = await pullService.ApprovePullOutAsync(pullReq.Id, approverRole: "staff", approverName: "StaffManager");
            Console.WriteLine($"Pull-Out status after approval: {approved.Status}, ApprovedBy: {approved.ApprovedBy}");

            var completed = await pullService.CompletePullOutAsync(pullReq.Id);
            Console.WriteLine($"Pull-Out after completion: {completed.Status}");

            updatedVar = await db.ShoeColorVariations.FindAsync(variation.Id);
            Console.WriteLine($"Variation stock after pull-out: {updatedVar.StockQuantity}");

            var bigPullDto = new CreatePullOutDto
            {
                ShoeColorVariationId = variation.Id,
                Quantity = 15,
                Reason = "Return to supplier",
                ReasonDetails = "Large return",
                RequestedBy = "StaffB"
            };

            var bigPull = await pullService.CreatePullOutRequestAsync(bigPullDto);
            Console.WriteLine($"\nCreated big Pull-Out request: ID {bigPull.Id}, Qty {bigPull.Quantity}, Status {bigPull.Status}");

            try
            {
                await pullService.ApprovePullOutAsync(bigPull.Id, approverRole: "staff", approverName: "StaffB");
                Console.WriteLine("Unexpected: staff approved big pull-out.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Expected approval failure for staff: {ex.Message}");
            }

            var bigApproved = await pullService.ApprovePullOutAsync(bigPull.Id, approverRole: "manager", approverName: "Manager1");
            Console.WriteLine($"Big Pull-Out approved by: {bigApproved.ApprovedBy} - Status: {bigApproved.Status}");

            try
            {
                var bigCompleted = await pullService.CompletePullOutAsync(bigPull.Id);
                Console.WriteLine($"Big Pull-Out completed. Status: {bigCompleted.Status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Completing big pull-out failed (likely insufficient stock): {ex.Message}");
            }

            var invReport = await report.GenerateInventoryReportAsync();
            Console.WriteLine("\n=== Inventory Report ===");
            Console.WriteLine($"Total SKUs (variations): {invReport.TotalSkuCount}");
            Console.WriteLine($"Total Units in Stock: {invReport.TotalUnitsInStock}");
            Console.WriteLine($"Total Inventory Value: {invReport.TotalInventoryValue:C}");
            Console.WriteLine($"Low Stock Count: {invReport.LowStockCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nUnhandled error in demo: {ex.GetType().Name} - {ex.Message}");
        }

        Console.WriteLine("\nDemo finished. Press any key to exit.");
        Console.ReadKey();
    }
}
