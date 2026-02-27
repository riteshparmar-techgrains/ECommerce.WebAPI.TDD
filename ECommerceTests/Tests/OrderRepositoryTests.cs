using Xunit;
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Data;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Repository;
using Microsoft.EntityFrameworkCore.InMemory;

namespace ECommerceTests.Tests
{
    public class OrderRepositoryTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_Should_Save_Order_To_Database()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new OrderRepository(context);

            var order = new Order
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100
            };

            // Act
            var orderId = await repository.AddAsync(order);

            // Assert
            var savedOrder = await context.Orders.FindAsync(orderId);

            Assert.NotNull(savedOrder);
            Assert.Equal(order.CustomerId, savedOrder.CustomerId);
            Assert.Equal(order.ProductId, savedOrder.ProductId);
            Assert.Equal(order.Quantity, savedOrder.Quantity);
            Assert.Equal(order.TotalAmount, savedOrder.TotalAmount);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Order()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new OrderRepository(context);
            var order = new Order
            {
                CustomerId = 1,
                ProductId = 1,
                Quantity = 2,
                TotalAmount = 100
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Act
            var retrievedOrder = await repository.GetByIdAsync(order.Id);

            // Assert
            Assert.NotNull(retrievedOrder);
            Assert.Equal(order.CustomerId, retrievedOrder.CustomerId);
            Assert.Equal(order.ProductId, retrievedOrder.ProductId);
            Assert.Equal(order.Quantity, retrievedOrder.Quantity);
            Assert.Equal(order.TotalAmount, retrievedOrder.TotalAmount);
        }
    }
}