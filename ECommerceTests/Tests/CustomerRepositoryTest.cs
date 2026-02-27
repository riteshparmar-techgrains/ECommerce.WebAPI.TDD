using ECommerceWebAPI.Data;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceTests.Tests
{
    public class CustomerRepositoryTest
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Customer()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new CustomerRepository(context);
            var customer = new Customer { Id = 1, Name = "Ritesh Parmar" };

            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1);
            
            // Assert
            Assert.Equal(customer, result);
        }

        [Fact]
        public async Task ExistsAsync_Should_Return_True_If_Customer_Exists()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new CustomerRepository(context);
            
            var customer = new Customer { Id = 1, Name = "Ritesh Parmar" };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            
            // Act
            var exists = await repository.ExistsAsync(1);
            
            // Assert
            Assert.True(exists);
        }
    }
}
