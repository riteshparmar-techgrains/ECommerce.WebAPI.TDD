using ECommerceWebAPI.Entities;

namespace ECommerceWebAPI.Repository
{
    public interface IOrderRepository
    {
        Task<int> AddAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task UpdateAsync(Order order);
    }
}
