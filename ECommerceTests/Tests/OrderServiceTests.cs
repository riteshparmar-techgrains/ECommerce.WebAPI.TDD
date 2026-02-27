using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Entities;
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
}