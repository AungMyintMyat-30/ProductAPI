using ProductCore.Entities;
using ProductCore.Models;

namespace ProductAPI.Interfaces
{
    public interface IProductService
    {
        Task<PaginationResultModel<List<Product>>> GetAllAsync(int skipRows, int pageSize);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
