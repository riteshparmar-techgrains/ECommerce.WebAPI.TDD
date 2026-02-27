using ECommerceWebAPI.Entities;

namespace ECommerceWebAPI.Repository
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task UpdateAsync(Product product);
    }
}
