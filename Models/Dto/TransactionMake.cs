using System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
class ValidateToOrService : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        int to = (int)validationContext.ObjectType.GetProperty("To")!.GetValue(validationContext.ObjectInstance, null)!;
        string service = (string)validationContext.ObjectType.GetProperty("Service")!.GetValue(validationContext.ObjectInstance, null)!;

        if (to == 0 && string.IsNullOrEmpty(service))
            return new ValidationResult(@"""To"" or ""Service"" field required.");

        return ValidationResult.Success!;
    }
}

namespace backend.Models.Dto
{
    public class TransactionMake
    {
        [ValidateToOrService]
        public int To { get; set; }
        public string Service { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        [Required]
        public uint Amount { get; set; }
    }
}