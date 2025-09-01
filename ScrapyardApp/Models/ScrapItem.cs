using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScrapyardApp.Models
{
    public class ScrapItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Item Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        [Display(Name = "Weight (kg)")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Price per kg is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per kg must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price per kg")]
        public decimal PricePerKg { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; } // Nullable to allow uncategorized items

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
    }
}