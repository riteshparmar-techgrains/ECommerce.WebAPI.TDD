using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Entities;

namespace ECommerceWebAPI.Services
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(CreateOrderRequest request);
        Task<Order> GetOrderByIdAsync(int id);
    }
}
