using ECommerceWebAPI.Enums;

namespace ECommerceWebAPI.Services
{
    public interface IOrderStatusService
    {
        Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
    }
}
