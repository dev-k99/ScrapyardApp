using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScrapyardApp.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; } // e.g., Create, Update, Delete

        [Required]
        [StringLength(50)]
        public string Entity { get; set; } // e.g., ScrapItem, Sale

        public int EntityId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Action Date")]
        [DataType(DataType.DateTime)]
        public DateTime ActionDate { get; set; }

        [StringLength(500)]
        public string Details { get; set; } // JSON or description of changes
    }
}