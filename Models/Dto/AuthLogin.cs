using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dto
{
    public class AuthLogin
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}