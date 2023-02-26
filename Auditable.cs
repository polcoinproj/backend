using System.Text.Json.Serialization;

public abstract class Auditable
{
    [JsonIgnore]
    public DateTimeOffset DateCreated { get; set; }
}