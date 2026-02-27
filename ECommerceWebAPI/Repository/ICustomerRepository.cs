using ECommerceWebAPI.Entities;

namespace ECommerceWebAPI.Repository
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
