using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductAPI.Services;
using ProductCore.Entities;
using ProductInfrastructure.Data;

namespace ProductAPI.Test
{
    public class ProductServiceTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private readonly ILoggerFactory _loggerFactory;

        public ProductServiceTests()
        {
            // Use an in-memory database for testing
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestProductDb")
                .Options;

            // Setup a loggerFactory.
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                _ = builder.AddConsole();
            });
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnPaginatedProducts()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            ApplicationDbContext context = new(options);
            context.Products.AddRange(new List<Product>
            {
                new() { StockNo = "S001", StockName = "Test1", Price = 50, Category = "A" },
                new() { StockNo = "S002", StockName = "Test2", Price = 60, Category = "B"  },
            });
            _ = await context.SaveChangesAsync();
            ILogger<ProductService> logger = _loggerFactory.CreateLogger<ProductService>();
            ProductService service = new(context, logger);

            ProductCore.Models.PaginationResultModel<List<Product>> result = await service.GetAllAsync(0, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.RecordsTotal);
            Assert.All(result.Records, product => Assert.NotEqual(0, product.Id));
        }

        [Fact]
        public async Task CreateAsync_Should_Add_Product()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            ApplicationDbContext context = new(options);
            ILogger<ProductService> logger = _loggerFactory.CreateLogger<ProductService>();
            ProductService service = new(context, logger);
            Product product = new() { StockNo = "S001", StockName = "Test Product", Price = 100, Category = "Test" };

            Product result = await service.CreateAsync(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("S001", result.StockNo);
            Assert.NotEqual(0, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            ApplicationDbContext context = new(options);
            Product productToAdd = new() { StockNo = "S001", StockName = "Existing Product", Price = 75, Category = "C" };
            _ = context.Products.Add(productToAdd);
            _ = await context.SaveChangesAsync();

            ILogger<ProductService> logger = _loggerFactory.CreateLogger<ProductService>();
            ProductService service = new(context, logger);

            Product? result = await service.GetByIdAsync(productToAdd.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productToAdd.Id, result.Id);
            Assert.Equal("S001", result.StockNo);
            Assert.Equal("Existing Product", result.StockName);
            Assert.Equal(75, result.Price);
            Assert.Equal("C", result.Category);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            ApplicationDbContext context = new(options);
            ILogger<ProductService> logger = _loggerFactory.CreateLogger<ProductService>();
            ProductService service = new(context, logger);
            int nonExistingId = 99;

            Product? result = await service.GetByIdAsync(nonExistingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnTrue_WhenProductIsUpdated()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            ApplicationDbContext context = new(options);
            Product initialProduct = new() { StockNo = "OLD123", StockName = "Old Name", Price = 25, Category = "Old" };
            _ = context.Products.Add(initialProduct);
            _ = await context.SaveChangesAsync();
            int productId = initialProduct.Id;

            ILogger<ProductService> logger = _loggerFactory.CreateLogger<ProductService>();
            ProductService service = new(context, logger);

            Product updatedProduct = new() { Id = productId, StockNo = "NEW456", StockName = "New Name", Price = 35, Category = "New" };

            bool result = await service.UpdateAsync(updatedProduct);
            Product? retrievedProduct = await context.Products.FindAsync(productId);

            // Assert
            Assert.True(result);
            Assert.NotNull(retrievedProduct);
            Assert.Equal("NEW456", retrievedProduct.StockNo);
            Assert.Equal("New Name", retrievedProduct.StockName);
            Assert.Equal(35, retrievedProduct.Price);
            Assert.Equal("New", retrievedProduct.Category);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnFalse_WhenProductDoesNotExist()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            ApplicationDbContext context = new(options);
            ILogger<ProductService> logger = _loggerFactory.CreateLogger<ProductService>();
            ProductService service = new(context, logger);
            Product nonExistingProduct = new() { Id = 99, StockNo = "TEST", StockName = "Test Product", Price = 10, Category = "T" };

            bool result = await service.UpdateAsync(nonExistingProduct);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_Should_Remove_Product()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            ApplicationDbContext context = new(options);
            Product productToDelete = new() { StockNo = "S002", StockName = "Product to Delete", Price = 50, Category = "B" };
            _ = context.Products.Add(productToDelete);
            _ = await context.SaveChangesAsync();

            ILogger<ProductService> logger = _loggerFactory.CreateLogger<ProductService>();
            ProductService service = new(context, logger);

            bool deleted = await service.DeleteAsync(productToDelete.Id);

            // Assert
            Assert.True(deleted);
        }
    }
}
