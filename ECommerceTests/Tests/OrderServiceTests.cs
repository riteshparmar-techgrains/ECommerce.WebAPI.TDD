using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Enums;
using ECommerceWebAPI.Expection;
using ECommerceWebAPI.Repository;
using ECommerceWebAPI.Services;
using Moq;
using Xunit;

public class OrderServiceTests
{
    private readonly Mock<ICustomerRepository> _customerRepo = new();
    private readonly Mock<IProductRepository> _productRepo = new();
    private readonly Mock<IOrderRepository> _orderRepo = new();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(
            _customerRepo.Object,
            _productRepo.Object,
            _orderRepo.Object);
    }

    // Starting to test CreateOrderAsync

    [Fact]
    public async Task CreateOrder_Should_Return_OrderId_When_All_Is_Valid()
    {
        // Arrage
        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            ProductId = 1,
            Quantity = 2
        };

        _customerRepo.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);

        _productRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Product
            {
                Id = 1,
                Price = 100,
                Stock = 5
            });

        _orderRepo.Setup(x => x.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync(10);

        // Act
        var result = await _service.CreateOrderAsync(request);

        // Assert
        Assert.Equal(10, result);
    }

    [Fact]
    public async Task CreateOrder_Should_Throw_When_Customer_Not_Found()
    {
        // Arrage
        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            ProductId = 1,
            Quantity = 2
        };
        _customerRepo.Setup(x => x.ExistsAsync(1)).ReturnsAsync(false);
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrder_Should_Throw_When_Product_Not_Found()
    {
        // Arrage

        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            ProductId = 99,
            Quantity = 2
        };

        _customerRepo.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
        _productRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Product)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrder_Should_Throw_When_Insufficient_Stock()
    {
        // Arrage
        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            ProductId = 1,
            Quantity = 2
        };

        _customerRepo.Setup(x => x.ExistsAsync(1)).ReturnsAsync(true);
        _productRepo.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(new Product
            {
                Id = 1,
                Price = 100,
                Stock = 1
            });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrder_Should_Throw_When_Quantity_Is_Invalid()
    {
        // Arrage
        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            ProductId = 1,
            Quantity = 0
        };
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrder_Should_Throw_When_Quantity_Is_Negative()
    {
        // Arrage
        var request = new CreateOrderRequest
        {
            CustomerId = 1,
            ProductId = 1,
            Quantity = -5
        };
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrder_Should_Throw_When_CustomerId_Is_Invalid()
    {
        // Arrage
        var request = new CreateOrderRequest
        {
            CustomerId = -1,
            ProductId = 1,
            Quantity = 2
        };
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    [Fact]
    public async Task CreateOrder_Should_Throw_When_CustomerId_Is_Zero()
    {
        // Arrage
        var request = new CreateOrderRequest
        {
            CustomerId = 0,
            ProductId = 1,
            Quantity = 2
        };
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateOrderAsync(request));
    }

    // End of CreateOrderAsync tests


    // Starting to test GetOrderByIdAsync

    [Fact]
    public async Task GetOrderById_Should_Return_Order_When_Found()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            CustomerId = 1,
            ProductId = 1,
            Quantity = 2
        };

        _orderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
        _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Customer { Id = 1, Name = "Ritesh" });
        _productRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Product { Id = 1, Name = "Laptop", Price = 1000 });

        // Act
        var result = await _service.GetOrderByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(2000m, result.TotalAmount);
    }

    [Fact]
    public async Task GetOrderById_Should_Throw_When_Order_Not_Found()
    {
        // Arrange
        _orderRepo.Setup(x => x.GetByIdAsync(99)).ReturnsAsync((Order)null);
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetOrderByIdAsync(99));
    }

    [Fact]
    public async Task GetOrderById_Should_Return_Correct_TotalAmount()
    {
        // Arrange
        var order = new Order { Id = 1, CustomerId = 1, ProductId = 1, Quantity = 3 };
        var customer = new Customer { Id = 1, Name = "Ritesh" };
        var product = new Product { Id = 1, Name = "Laptop", Price = 500 };

        _orderRepo.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(order);
        _customerRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(customer);
        _productRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _service.GetOrderByIdAsync(10);

        // Assert
        Assert.NotNull(result);
    }

    // End of GetOrderByIdAsync tests


    // Starting to test Update Order Status

    [Fact]
    public async Task UpdateOrderStatusAsync_OrderExists_UpdatesStatus()
    {
        var order = new Order { Id = 1, Status = OrderStatus.Pending };

        var mockOrderRepo = new Mock<IOrderRepository>();
        mockOrderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);

        var service = new OrderStatusService(mockOrderRepo.Object);

        await service.UpdateOrderStatusAsync(1, OrderStatus.Shipped);

        Assert.Equal(OrderStatus.Shipped, order.Status);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_OrderDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var mockOrderRepo = new Mock<IOrderRepository>();

        mockOrderRepo.Setup(x => x.GetByIdAsync(163)).ReturnsAsync((Order)null);
        var service = new OrderStatusService(mockOrderRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateOrderStatusAsync(163, OrderStatus.Shipped));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_InvalidTransition_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        var mockOrderRepo = new Mock<IOrderRepository>();

        mockOrderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
        var service = new OrderStatusService(mockOrderRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderStatusAsync(1, OrderStatus.Delivered));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_ValidTransition_UpdatesStatus()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Shipped };
        var mockOrderRepo = new Mock<IOrderRepository>();

        mockOrderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
        var service = new OrderStatusService(mockOrderRepo.Object);
        // Act
        await service.UpdateOrderStatusAsync(1, OrderStatus.Delivered);
        // Assert
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_SameStatus_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        var mockOrderRepo = new Mock<IOrderRepository>();

        mockOrderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
        var service = new OrderStatusService(mockOrderRepo.Object);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderStatusAsync(1, OrderStatus.Pending));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_FromDeliveredToShipped_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Delivered };
        var mockOrderRepo = new Mock<IOrderRepository>();

        mockOrderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
        var service = new OrderStatusService(mockOrderRepo.Object);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderStatusAsync(1, OrderStatus.Shipped));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_FromShippedToPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Shipped };
        var mockOrderRepo = new Mock<IOrderRepository>();

        mockOrderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
        var service = new OrderStatusService(mockOrderRepo.Object);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderStatusAsync(1, OrderStatus.Pending));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_FromPendingToDelivered_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Pending };
        var mockOrderRepo = new Mock<IOrderRepository>();

        mockOrderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
        var service = new OrderStatusService(mockOrderRepo.Object);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderStatusAsync(1, OrderStatus.Delivered));
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_FromDeliveredToPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var order = new Order { Id = 1, Status = OrderStatus.Delivered };
        var mockOrderRepo = new Mock<IOrderRepository>();

        mockOrderRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(order);
        var service = new OrderStatusService(mockOrderRepo.Object);
        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateOrderStatusAsync(1, OrderStatus.Pending));
    }
}