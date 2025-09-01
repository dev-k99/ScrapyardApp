using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScrapyardApp.Models
{
    public class PriceHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ScrapItemId { get; set; }

        [ForeignKey("ScrapItemId")]
        public ScrapItem ScrapItem { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Price per kg")]
        public decimal PricePerKg { get; set; }

        [Required]
        [Display(Name = "Effective Date")]
        [DataType(DataType.DateTime)]
        public DateTime EffectiveDate { get; set; }
    }
}