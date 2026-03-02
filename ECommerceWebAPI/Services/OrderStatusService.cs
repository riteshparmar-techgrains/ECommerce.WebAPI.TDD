using ECommerceWebAPI.Enums;
using ECommerceWebAPI.Expection;
using ECommerceWebAPI.Repository;

namespace ECommerceWebAPI.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IOrderRepository _orderRepository;
        public OrderStatusService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
                throw new NotFoundException($"Order {orderId} not found");

            if (!IsValidTransition(order.Status, newStatus))
                throw new InvalidOperationException("Invalid status transition");

            order.Status = newStatus;

            await _orderRepository.UpdateAsync(order);
        }

        private bool IsValidTransition(OrderStatus current, OrderStatus next) => (current == OrderStatus.Pending && next == OrderStatus.Shipped) ||
                   (current == OrderStatus.Shipped && next == OrderStatus.Delivered);
    }
}
