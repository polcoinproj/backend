namespace backend.Models
{
    public class School : Auditable
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}