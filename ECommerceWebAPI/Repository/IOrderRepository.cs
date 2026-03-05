using ECommerceWebAPI.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebAPI.Repository
{
    public interface IOrderRepository
    {
        Task<int> AddAsync(Order order);
        Task<Order?> GetByIdAsync(int id);
        Task UpdateAsync(Order order);
        Task CancelOrderAsync(int id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
    }
}
