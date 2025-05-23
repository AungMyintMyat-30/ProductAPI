﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Controllers;
using ProductAPI.Interfaces;
using ProductCore.Entities;
using ProductCore.Models;

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
            List<Product> products = [new Product { Id = 1, StockName = "TestProduct" }];
            PaginationResultModel<List<Product>> paginationResult = new(1, products);
            _ = _mockService.Setup(s => s.GetAllAsync(0, 10)).ReturnsAsync(paginationResult);

            IActionResult result = await _controller.GetAll(0, 10);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WhenProductExists()
        {
            Product product = new() { Id = 1, StockName = "Product1" };
            _ = _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

            IActionResult result = await _controller.GetById(1);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenProductDoesNotExist()
        {
            _ = _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Product?)null);

            IActionResult result = await _controller.GetById(1);

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsCreatedResult_WhenProductIsCreated()
        {
            Product product = new() { Id = 1, StockName = "NewProduct" };
            _ = _mockService.Setup(s => s.GetByIdAsync(product.Id)).ReturnsAsync((Product?)null);
            _ = _mockService.Setup(s => s.CreateAsync(product)).ReturnsAsync(product);

            IActionResult result = await _controller.Create(product);

            // Assert
            CreatedResult createdResult = Assert.IsType<CreatedResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenProductExists()
        {
            Product product = new() { Id = 1, StockName = "DuplicateProduct" };
            _ = _mockService.Setup(s => s.GetByIdAsync(product.Id)).ReturnsAsync(product);

            IActionResult result = await _controller.Create(product);

            // Assert
            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WhenProductUpdated()
        {
            Product product = new() { Id = 1, StockName = "UpdatedProduct" };
            _ = _mockService.Setup(s => s.UpdateAsync(product)).ReturnsAsync(true);

            IActionResult result = await _controller.Update(1, product);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenIdMismatch()
        {
            Product product = new() { Id = 2, StockName = "MismatchProduct" };

            IActionResult result = await _controller.Update(1, product);

            // Assert
            BadRequestObjectResult badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenProductNotFound()
        {
            Product product = new() { Id = 1, StockName = "NonExistingProduct" };
            _ = _mockService.Setup(s => s.UpdateAsync(product)).ReturnsAsync(false);

            IActionResult result = await _controller.Update(1, product);

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenProductDeleted()
        {
            _ = _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            IActionResult result = await _controller.Delete(1);

            // Assert
            OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenProductNotFound()
        {
            _ = _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(false);

            IActionResult result = await _controller.Delete(1);

            // Assert
            NotFoundObjectResult notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
