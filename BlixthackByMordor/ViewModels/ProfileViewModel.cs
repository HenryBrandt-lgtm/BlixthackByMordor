using System.ComponentModel.DataAnnotations;
using BlixthackByMordor.Models;

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

        [MaxLength(500)]
        [Display(Name = "About me")]
        [DataType(DataType.MultilineText)]
        public string AboutMe { get; set; } = string.Empty;

        public bool IsOwner { get; set; }

        public IReadOnlyList<ProfileThreadItem> Threads { get; set; } = [];

        public IReadOnlyList<ProfileAnswerItem> Answers { get; set; } = [];

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

        public static ProfileViewModel From(UserModel user, bool isOwner)
        {
            return new ProfileViewModel
            {
                Username = user.Username,
                Email = isOwner ? user.Email : string.Empty,
                AboutMe = user.AboutMe,
                CreatedAt = user.CreatedAt,
                IsOwner = isOwner,
                Threads = MapThreads(user),
                Answers = MapAnswers(user)
            };
        }

        public void AttachActivity(UserModel user)
        {
            Threads = MapThreads(user);
            Answers = MapAnswers(user);
        }

        private static IReadOnlyList<ProfileThreadItem> MapThreads(UserModel user)
        {
            return (user.Threads ?? [])
                .OrderByDescending(thread => thread.CreatedAt)
                .Select(thread => new ProfileThreadItem
                {
                    Id = thread.Id,
                    Title = thread.Title,
                    CategoryName = thread.Category.Name,
                    CreatedAt = thread.CreatedAt
                })
                .ToList();
        }

        private static IReadOnlyList<ProfileAnswerItem> MapAnswers(UserModel user)
        {
            return (user.Answers ?? [])
                .Where(answer => answer.Thread != null)
                .OrderByDescending(answer => answer.CreatedAt)
                .Select(answer => new ProfileAnswerItem
                {
                    ThreadId = answer.ThreadId,
                    ThreadTitle = answer.Thread!.Title,
                    Excerpt = Excerpt(answer.Content),
                    CreatedAt = answer.CreatedAt
                })
                .ToList();
        }

        private static string Excerpt(string content, int maxLength = 140)
        {
            content = content.Trim();
            if (content.Length <= maxLength)
            {
                return content;
            }

            return content[..maxLength].TrimEnd() + "…";
        }
    }

    public class ProfileThreadItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ProfileAnswerItem
    {
        public int ThreadId { get; set; }
        public string ThreadTitle { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
