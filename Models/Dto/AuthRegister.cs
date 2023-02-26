using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dto
{
    public class AuthRegister
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int School { get; set; }
        [Required]
        public string Grade { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}