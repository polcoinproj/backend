using System.Text.Json.Serialization;

namespace backend.Models
{
    public static class UserRoles
    {
        public const string User = "user";
        public const string Admin = "admin";
        public const string SuperUser = "su";

        public const string Administration = Admin + "," + SuperUser;
    }

    public class User : Auditable
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public School School { get; set; } = new School();
        public string Grade { get; set; } = string.Empty;

        [JsonIgnore]
        public byte[] PasswordHash { get; set; } = new byte[] { };
        [JsonIgnore]
        public byte[] PasswordSalt { get; set; } = new byte[] { };

        [JsonIgnore]
        public bool Verified { get; set; }
        public string Role { get; set; } = string.Empty;

        public int Balance { get; set; }
    }
}
