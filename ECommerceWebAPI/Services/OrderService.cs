using Azure.Core;
using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Enums;
using ECommerceWebAPI.Expection;
using ECommerceWebAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceWebAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;

        public OrderService(
            ICustomerRepository customerRepo,
            IProductRepository productRepo,
            IOrderRepository orderRepo)
        {
            _customerRepo = customerRepo;
            _productRepo = productRepo;
            _orderRepo = orderRepo;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            var customer = await _customerRepo.GetByIdAsync(order.CustomerId);

            if (customer == null)
                throw new NotFoundException($"Customer {order.CustomerId} not found");

            var product = await _productRepo.GetByIdAsync(order.ProductId);

            if (product == null)
                throw new NotFoundException($"Product {order.ProductId} not found");

            // Only recalculate total if needed
            order.TotalAmount = product.Price * order.Quantity;

            return order;
        }

        public async Task<int> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request.CustomerId <= 0)
                throw new ArgumentException("CustomerId must be greater than zero");

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            var customerExists = await _customerRepo.ExistsAsync(request.CustomerId);
            if (!customerExists)
                throw new NotFoundException("Customer not found");

            var product = await _productRepo.GetByIdAsync(request.ProductId);
            if (product == null)
                throw new NotFoundException("Product not found");

            if (product.Stock < request.Quantity)
                throw new InvalidOperationException("Insufficient stock");

            product.Stock -= request.Quantity;

            var order = new Order
            {
                CustomerId = request.CustomerId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                TotalAmount = product.Price * request.Quantity
            };

            await _productRepo.UpdateAsync(product);

            return await _orderRepo.AddAsync(order);
        }

        public async Task CancelOrderAsync(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException($"Order {id} not found");

            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be cancelled");

            if (order.Status == OrderStatus.Cancelled)
                throw new InvalidOperationException("Order is already cancelled");

            order.Status = OrderStatus.Cancelled;

            await _orderRepo.UpdateAsync(order);
        }

        public Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepo.GetAllOrdersAsync();
        }
    }
}
