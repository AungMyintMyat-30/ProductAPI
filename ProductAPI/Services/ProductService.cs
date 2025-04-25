using Microsoft.EntityFrameworkCore;
using ProductAPI.Interfaces;
using ProductCore.Entities;
using ProductCore.Models;
using ProductInfrastructure.Data;
using System.Collections.Generic;

namespace ProductAPI.Services;
public class ProductService(ApplicationDbContext context) : IProductService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PaginationResultModel<List<Product>>> GetAllAsync(int skipRows, int pageSize)
    {
        var query = _context.Products.AsNoTracking();

        int totalRecords = await query.CountAsync();
        List<Product> records = await query
            .Skip(skipRows)
            .Take(pageSize)
            .ToListAsync();

        return new PaginationResultModel<List<Product>>(totalRecords, records);
    }

    public async Task<Product?> GetByIdAsync(int id) =>
    await _context.Products.FindAsync(id);

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> UpdateAsync(Product product)
    {
        var result = await _context.Products.FindAsync(product.Id);
        if (result == null) return false;

        result.StockNo = product.StockNo;
        result.StockName = product.StockName;
        result.Price = product.Price;
        result.Category = product.Category;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
