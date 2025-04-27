using System.ComponentModel.DataAnnotations;

namespace ProductCore.Entities
{
    public class Product
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string? StockNo { get; set; }

        [Required]
        [StringLength(200)]
        public string? StockName { get; set; }

        [Required]
        [Range(0.0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public double Price { get; set; }

        [Required]
        [StringLength(200)]
        public string? Category { get; set; }
    }
}
