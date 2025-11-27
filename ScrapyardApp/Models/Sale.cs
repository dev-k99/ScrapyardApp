using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScrapyardApp.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Scrap item is required")]
        [Display(Name = "Scrap Item")]
        public int ScrapItemId { get; set; }

        [ForeignKey("ScrapItemId")]
        public ScrapItem ScrapItem { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [Required(ErrorMessage = "Weight sold is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight sold must be greater than 0")]
        [Display(Name = "Weight Sold (kg)")]
        public double WeightSold { get; set; }

        [Required(ErrorMessage = "Total price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }

        [Required]
        [Display(Name = "Sale Date")]
        [DataType(DataType.DateTime)]
        public DateTime SaleDate { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters")]
        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } // e.g., Cash, Credit, Bank Transfer

        [Required(ErrorMessage = "Invoice number is required")]
        [StringLength(50, ErrorMessage = "Invoice number cannot exceed 50 characters")]
        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; } // Unique identifier for the sale

        //public bool IsActive { get; set; } = true; // Default to true for active sales
    }
}