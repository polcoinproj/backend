namespace backend.Models
{
    public class VerifyEntry : Auditable
    {
        public int Id { get; set; }

        public User User { get; set; } = new User { };

        public int Code { get; set; }
    }
}