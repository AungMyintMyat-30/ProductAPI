using Microsoft.EntityFrameworkCore;
using ProductCore.Entities;

namespace ProductInfrastructure.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options) { }

        public DbSet<Product> Products => Set<Product>();
    }
}
