using System.ComponentModel.DataAnnotations;

namespace ScrapyardApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required][StringLength(100)] public string FullName { get; set; } = string.Empty;
    }
}