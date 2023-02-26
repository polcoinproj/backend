namespace backend.Models
{
    public class Transaction : Auditable
    {
        public int Id { get; set; }

        public User From { get; set; } = new User();

        public User To { get; set; } = new User();
        public string Service { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public int Amount { get; set; }
    }
}
