using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dto
{
    public class ContactCreateDto
    {
        [Required]
        public int Target { get; set; }
    }
}