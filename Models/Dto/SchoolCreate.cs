using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dto
{
    public class SchoolCreate
    {
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}