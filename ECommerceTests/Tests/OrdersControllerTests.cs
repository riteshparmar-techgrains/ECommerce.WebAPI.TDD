using ECommerceWebAPI.Controllers;
using ECommerceWebAPI.DTOs;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Expection;
using ECommerceWebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceTests.Tests
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _serviceMock;
        private readonly Mock<IOrderStatusService> _statusServiceMock;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _serviceMock = new Mock<IOrderService>();
            _statusServiceMock = new Mock<IOrderStatusService>();
            _controller = new OrdersController(_serviceMock.Object,_statusServiceMock.Object);
        }


        [Fact]
        public async Task Create_Should_Return_201_When_Order_Created()
        {
            var request = new CreateOrderRequest();

            _serviceMock
                .Setup(x => x.CreateOrderAsync(request))
                .ReturnsAsync(10);

            var result = await _controller.Create(request);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(nameof(_controller.GetOrderById), createdResult.ActionName);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task Create_Should_Return_404_When_NotFoundException_Is_Thrown()
        {
            // Arrange
            var request = new CreateOrderRequest();

            _serviceMock.Setup(s => s.CreateOrderAsync(request)).ThrowsAsync(new NotFoundException("Not found"));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Not found", notFoundResult.Value);
        }

        [Fact]
        public async Task Create_Should_Return_400_When_InvalidOprationException_Thrown()
        {
            // Arrange
            var request = new CreateOrderRequest();

            _serviceMock.Setup(s => s.CreateOrderAsync(request)).ThrowsAsync(new InvalidOperationException("Stock not available"));

            // Act
            var result = await _controller.Create(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Stock not available", badRequestResult.Value);
        }

        [Fact]
        public async Task GetOrderById_Should_Return_200_When_Order_Found()
        {
            // Arrange
            int orderId = 1;
            var order = new Order { Id = orderId };

            _serviceMock.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(order, okResult.Value);
        }

        [Fact]
        public async Task GetOrderById_Should_Return_404_When_KeyNotFoundException_Is_Thrown()
        {
            // Arrange
            int orderId = 1;

            _serviceMock.Setup(s => s.GetOrderByIdAsync(orderId)).ThrowsAsync(new KeyNotFoundException("Order not found"));

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Order not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetOrderById_Should_Return_500_When_Exception_Is_Thrown()
        {
            // Arrange
            int orderId = 1;

            _serviceMock.Setup(s => s.GetOrderByIdAsync(orderId)).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetOrderById(orderId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Database error", statusCodeResult.Value);
        }
    }
}