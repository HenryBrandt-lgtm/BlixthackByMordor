using System.ComponentModel.DataAnnotations;

namespace BlixthackByMordor.ViewModels
{
    public class ProfileViewModel : IValidatableObject
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Member since")]
        public DateTime CreatedAt { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string? ConfirmNewPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var changingPassword = !string.IsNullOrWhiteSpace(NewPassword)
                || !string.IsNullOrWhiteSpace(ConfirmNewPassword)
                || !string.IsNullOrWhiteSpace(CurrentPassword);

            if (!changingPassword)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                yield return new ValidationResult(
                    "Current password is required to change your password.",
                    [nameof(CurrentPassword)]);
            }

            if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
            {
                yield return new ValidationResult(
                    "New password must be at least 6 characters.",
                    [nameof(NewPassword)]);
            }

            if (NewPassword != ConfirmNewPassword)
            {
                yield return new ValidationResult(
                    "The new passwords do not match.",
                    [nameof(ConfirmNewPassword)]);
            }
        }
    }
}
