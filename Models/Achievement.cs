namespace backend.Models
{
    public enum Status
    {
        InProcess = 1,
        Approved,
        Denied,
    }

    public class Achievement
    {
        public int Id { get; set; }

        public User User { get; set; } = new User();
        public Status Status { get; set; }

        public string Comment { get; set; } = string.Empty;
        public string[] Attachments { get; set; } = new string[] { };
    }
}