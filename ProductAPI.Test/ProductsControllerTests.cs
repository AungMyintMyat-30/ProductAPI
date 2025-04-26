using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Controllers;
using ProductAPI.Interfaces;
using ProductCore.Entities;
using ProductCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPI.Test
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly Mock<ILogger<ProductsController>> _loggerMock;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockService.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithProducts()
        {
            // Arrange
            var products = new List<Product> { new Product { Id = 1, StockName = "TestProduct" } };
            var paginationResult = new PaginationResultModel<List<Product>>(1, products);
            _mockService.Setup(s => s.GetAllAsync(0, 10)).ReturnsAsync(paginationResult);

            // Act
            var result = await _controller.GetAll(0, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, StockName = "Product1" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Product?)null);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult_WhenProductIsCreated()
        {
            // Arrange
            var product = new Product { Id = 1, StockName = "NewProduct" };
            _mockService.Setup(s => s.GetByIdAsync(product.Id)).ReturnsAsync((Product?)null);
            _mockService.Setup(s => s.CreateAsync(product)).ReturnsAsync(product);

            // Act
            var result = await _controller.Create(product);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result); // <-- Correct type here
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, StockName = "DuplicateProduct" };
            _mockService.Setup(s => s.GetByIdAsync(product.Id)).ReturnsAsync(product);

            // Act
            var result = await _controller.Create(product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenProductUpdated()
        {
            // Arrange
            var product = new Product { Id = 1, StockName = "UpdatedProduct" };
            _mockService.Setup(s => s.UpdateAsync(product)).ReturnsAsync(true);

            // Act
            var result = await _controller.Update(1, product);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            var product = new Product { Id = 2, StockName = "MismatchProduct" };

            // Act
            var result = await _controller.Update(1, product);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            var product = new Product { Id = 1, StockName = "NonExistingProduct" };
            _mockService.Setup(s => s.UpdateAsync(product)).ReturnsAsync(false);

            // Act
            var result = await _controller.Update(1, product);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenProductDeleted()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
