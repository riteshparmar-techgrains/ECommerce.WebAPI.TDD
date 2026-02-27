using ECommerceWebAPI.Data;
using ECommerceWebAPI.Entities;
using ECommerceWebAPI.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerceTests.Tests
{
    public class ProductRepositoryTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB per test
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task UpdateAsync_Should_Update_Product_In_Database()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new ProductRepository(context);

            var product = new Product { Id = 1, Name = "Test Product", Price = 10.0m };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            product.Name = "Updated Product";
            await repository.UpdateAsync(product);

            // Assert
            var updatedProduct = await context.Products.FindAsync(1);

            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Product", updatedProduct.Name);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Correct_Product()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new ProductRepository(context);
            var product = new Product { Id = 1, Name = "Test Product", Price = 10.0m };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var retrievedProduct = await repository.GetByIdAsync(1);

            // Assert
            Assert.NotNull(retrievedProduct);
            Assert.Equal("Test Product", retrievedProduct.Name);
        }

        [Fact]
        public async Task GetByIdAsync_Should_Return_Null_For_Nonexistent_Product()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new ProductRepository(context);

            // Act
            var retrievedProduct = await repository.GetByIdAsync(999); // non-existent ID

            // Assert
            Assert.Null(retrievedProduct);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_For_Nonexistent_Product()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new ProductRepository(context);
            var product = new Product { Id = 999, Name = "Nonexistent Product", Price = 10.0m };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => repository.UpdateAsync(product));
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_For_Null_Product()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new ProductRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateAsync(null!));
        }

        [Fact]
        public async Task GetByIdAsync_Should_Throw_Exception_For_Negative_Id()
        {
            // Arrange
            var context = CreateDbContext();
            var repository = new ProductRepository(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => repository.GetByIdAsync(-1));
        }
    }
}
