using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShoeShop.Repository.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Brand = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShoeColorVariations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShoeId = table.Column<int>(type: "INTEGER", nullable: false),
                    ColorName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    HexCode = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    StockQuantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ReorderLevel = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoeColorVariations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoeColorVariations_Shoes_ShoeId",
                        column: x => x.ShoeId,
                        principalTable: "Shoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderNumber = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExpectedDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockPullOuts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShoeColorVariationId = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ReasonDetails = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    RequestedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ApprovedBy = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PullOutDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPullOuts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockPullOuts_ShoeColorVariations_ShoeColorVariationId",
                        column: x => x.ShoeColorVariationId,
                        principalTable: "ShoeColorVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PurchaseOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ShoeColorVariationId = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityOrdered = table.Column<int>(type: "INTEGER", nullable: false),
                    QuantityReceived = table.Column<int>(type: "INTEGER", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_ShoeColorVariations_ShoeColorVariationId",
                        column: x => x.ShoeColorVariationId,
                        principalTable: "ShoeColorVariations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Shoes",
                columns: new[] { "Id", "Brand", "Cost", "CreatedDate", "Description", "ImageUrl", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Nike", 85.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Comfortable running shoes with air cushioning", null, true, "Air Max 270", 150.00m },
                    { 2, "Adidas", 95.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Energy-returning running shoes", null, true, "Ultra Boost 22", 180.00m },
                    { 3, "Puma", 45.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Iconic suede sneakers", null, true, "Suede Classic", 80.00m },
                    { 4, "Nike", 120.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classic basketball shoes", null, true, "Air Jordan 1", 200.00m },
                    { 5, "Adidas", 60.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classic shell-toe design", null, true, "Superstar", 100.00m },
                    { 6, "Puma", 70.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bold and chunky sneakers", null, true, "RS-X", 120.00m },
                    { 7, "Nike", 50.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Retro running shoes", null, true, "Cortez", 90.00m },
                    { 8, "Adidas", 55.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Minimalist tennis shoes", null, true, "Stan Smith", 95.00m },
                    { 9, "Puma", 80.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Performance basketball shoes", null, true, "Clyde Court", 140.00m },
                    { 10, "Nike", 65.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classic high-top sneakers", null, true, "Blazer Mid", 110.00m },
                    { 11, "Adidas", 90.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Modern nomad sneakers", null, true, "NMD R1", 160.00m },
                    { 12, "Puma", 55.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Retro-futuristic running shoes", null, true, "Future Rider", 95.00m },
                    { 13, "Nike", 75.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Versatile skateboarding shoes", null, true, "Dunk Low", 130.00m },
                    { 14, "Adidas", 150.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Premium lifestyle sneakers", null, true, "Yeezy Boost 350", 250.00m },
                    { 15, "Puma", 60.00m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Motorsport-inspired shoes", null, true, "Speedcat", 105.00m }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Address", "ContactEmail", "ContactPhone", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "123 Nike Street, Portland, OR", "orders@nikewholesale.com", "555-0101", true, "Nike Wholesale" },
                    { 2, "456 Adidas Ave, Portland, OR", "sales@adidascorp.com", "555-0102", true, "Adidas Distribution" },
                    { 3, "789 Puma Blvd, Portland, OR", "info@pumaint.com", "555-0103", true, "Puma International" }
                });

            migrationBuilder.InsertData(
                table: "ShoeColorVariations",
                columns: new[] { "Id", "ColorName", "HexCode", "IsActive", "ReorderLevel", "ShoeId", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Black", "#000000", true, 5, 1, 25 },
                    { 2, "White", "#FFFFFF", true, 5, 1, 30 },
                    { 3, "Red", "#FF0000", true, 5, 1, 15 },
                    { 4, "Blue", "#0000FF", true, 5, 2, 20 },
                    { 5, "Grey", "#808080", true, 5, 2, 18 },
                    { 6, "Navy", "#000080", true, 5, 3, 12 },
                    { 7, "Red", "#FF0000", true, 5, 3, 10 },
                    { 8, "Chicago Red", "#CD5C5C", true, 5, 4, 8 },
                    { 9, "Black", "#000000", true, 5, 4, 3 },
                    { 10, "White", "#FFFFFF", true, 5, 5, 35 },
                    { 11, "Black", "#000000", true, 5, 5, 22 },
                    { 12, "Multi", "#FF6347", true, 5, 6, 14 },
                    { 13, "White/Red", "#FFFFFF", true, 5, 7, 16 },
                    { 14, "White/Green", "#FFFFFF", true, 5, 8, 28 },
                    { 15, "Black", "#000000", true, 5, 9, 10 },
                    { 16, "White", "#FFFFFF", true, 5, 10, 20 },
                    { 17, "Black/Blue", "#000000", true, 5, 11, 15 },
                    { 18, "Grey/Yellow", "#808080", true, 5, 12, 18 },
                    { 19, "Panda", "#000000", true, 5, 13, 5 },
                    { 20, "Cream", "#FFFDD0", true, 5, 14, 4 },
                    { 21, "Black/Red", "#000000", true, 5, 15, 12 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderId",
                table: "PurchaseOrderItems",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_ShoeColorVariationId",
                table: "PurchaseOrderItems",
                column: "ShoeColorVariationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                table: "PurchaseOrders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoeColorVariations_ShoeId",
                table: "ShoeColorVariations",
                column: "ShoeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockPullOuts_ShoeColorVariationId",
                table: "StockPullOuts",
                column: "ShoeColorVariationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "StockPullOuts");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "ShoeColorVariations");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Shoes");
        }
    }
}
