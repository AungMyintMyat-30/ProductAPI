using ProductCore.Entities;
using ProductCore.Models;

namespace ProductAPI.Services;
public class ProductService(ApplicationDbContext context, ILogger<ProductService> logger) : IProductService
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<ProductService> _logger = logger;

    public async Task<PaginationResultModel<List<Product>>> GetAllAsync(int skipRows, int pageSize)
    {
        try
        {
            IQueryable<Product> query = _context.Products.AsNoTracking();
            int totalRecords = await query.CountAsync();
            List<Product> records = await query
                .Skip(skipRows)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResultModel<List<Product>>(totalRecords, records);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting all products.");
            throw;
        }
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Products.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while getting product by id: {id}");
            throw;
        }
    }

    public async Task<Product> CreateAsync(Product product)
    {
        try
        {
            _ = _context.Products.Add(product);
            _ = await _context.SaveChangesAsync();
            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while creating product: {product.StockName}");
            throw;
        }
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        try
        {
            Product? result = await _context.Products.FindAsync(product.Id);
            if (result == null) return false;

            result.StockNo = product.StockNo;
            result.StockName = product.StockName;
            result.Price = product.Price;
            result.Category = product.Category;
            _ = await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while updating product: {product.Id}, {product.StockName}");
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            Product? product = await _context.Products.FindAsync(id);
            if (product == null) return false;
            _ = _context.Products.Remove(product);
            _ = await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error occurred while deleting product with id: {id}");
            throw;
        }
    }
}
