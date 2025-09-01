using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ScrapyardApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(50, ErrorMessage = "Category name cannot exceed 50 characters")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } // e.g., Ferrous, Non-Ferrous

        public List<ScrapItem> ScrapItems { get; set; }
    }
}