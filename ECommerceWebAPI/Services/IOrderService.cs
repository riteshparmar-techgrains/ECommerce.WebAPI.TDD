using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Enums;

namespace ECommerceWebAPI.Services
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(CreateOrderRequest request);
        Task<Order> GetOrderByIdAsync(int id);
        Task UpdateOrderStatusAsync(int orderId,OrderStatus newStatus);
    }
}
