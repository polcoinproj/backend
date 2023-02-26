using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Contact : Auditable
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }
        public User Owner { get; set; } = new User();
        public User Target { get; set; } = new User();
    }
}
