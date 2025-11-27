using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScrapyardApp.Models
{
    public class Purchase
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Scrap item is required")]
        [Display(Name = "Scrap Item")]
        public int ScrapItemId { get; set; }

        [ForeignKey("ScrapItemId")]
        public ScrapItem ScrapItem { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        [Display(Name = "Weight Purchased (kg)")]
        public double WeightPurchased { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }

        [Required]
        [Display(Name = "Purchase Date")]
        [DataType(DataType.DateTime)]
        public DateTime PurchaseDate { get; set; }

        [StringLength(100, ErrorMessage = "Supplier cannot exceed 100 characters")]
        public string Supplier { get; set; } // Optional: Name of supplier

        //// ForSoftDelete
        //public bool IsActive { get; set; } = true; // Default to true for active purchases
    }
}