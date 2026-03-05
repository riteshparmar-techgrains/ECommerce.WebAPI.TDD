using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebAPI.Services
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(CreateOrderRequest request);
        Task<Order> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task UpdateOrderStatusAsync(int orderId,OrderStatus newStatus);
        Task CancelOrderAsync(int id);
    }
}
