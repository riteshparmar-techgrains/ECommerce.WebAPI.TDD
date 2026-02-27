using ECommerceWebAPI.Data;
using ECommerceWebAPI.Entities;
using Microsoft.CodeAnalysis;

namespace ECommerceWebAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Product Id must be greater than zero", nameof(id));

            return await _context.Products.FindAsync(id);
        }

        public async Task UpdateAsync(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);

            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }


    }
}
