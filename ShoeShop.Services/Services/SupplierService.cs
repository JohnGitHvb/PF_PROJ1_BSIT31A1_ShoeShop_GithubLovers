using Microsoft.EntityFrameworkCore;
using ShoeShop.Repository;
using ShoeShop.Repository.Entities;
using ShoeShop.Services.DTOs;
using ShoeShop.Services.Interfaces;

namespace ShoeShop.Services.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ShoeShopDbContext _context;

        public SupplierService(ShoeShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupplierDto>> GetAllSuppliersAsync()
        {
            var suppliers = await _context.Suppliers.ToListAsync();

            return suppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactEmail = s.ContactEmail,
                ContactPhone = s.ContactPhone,
                Address = s.Address,
                IsActive = s.IsActive
            }).ToList();
        }

        public async Task<List<SupplierDto>> GetActiveSuppliersAsync()
        {
            var suppliers = await _context.Suppliers
                .Where(s => s.IsActive)
                .ToListAsync();

            return suppliers.Select(s => new SupplierDto
            {
                Id = s.Id,
                Name = s.Name,
                ContactEmail = s.ContactEmail,
                ContactPhone = s.ContactPhone,
                Address = s.Address,
                IsActive = s.IsActive
            }).ToList();
        }

        public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return null;

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactEmail = supplier.ContactEmail,
                ContactPhone = supplier.ContactPhone,
                Address = supplier.Address,
                IsActive = supplier.IsActive
            };
        }

        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto)
        {
            var supplier = new Supplier
            {
                Name = dto.Name,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                Address = dto.Address,
                IsActive = true
            };

            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactEmail = supplier.ContactEmail,
                ContactPhone = supplier.ContactPhone,
                Address = supplier.Address,
                IsActive = supplier.IsActive
            };
        }

        public async Task<SupplierDto> UpdateSupplierAsync(int id, CreateSupplierDto dto)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null)
                throw new Exception($"Supplier with ID {id} not found");

            supplier.Name = dto.Name;
            supplier.ContactEmail = dto.ContactEmail;
            supplier.ContactPhone = dto.ContactPhone;
            supplier.Address = dto.Address;

            await _context.SaveChangesAsync();

            return new SupplierDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactEmail = supplier.ContactEmail,
                ContactPhone = supplier.ContactPhone,
                Address = supplier.Address,
                IsActive = supplier.IsActive
            };
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            // Check if supplier has any purchase orders
            var hasPurchaseOrders = await _context.PurchaseOrders
                .AnyAsync(po => po.SupplierId == id);

            if (hasPurchaseOrders)
            {
                throw new Exception("Cannot delete supplier with existing purchase orders. Deactivate instead.");
            }

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateSupplierAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier == null) return false;

            supplier.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}