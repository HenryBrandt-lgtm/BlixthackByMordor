using System.ComponentModel.DataAnnotations;

namespace BlixthackByMordor.Models;

public class UserModel
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;

    [Required, MinLength(1), MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(1), MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required] public DateTime CreatedAt { get; set; }

    [MaxLength(500)]
    public string AboutMe { get; set; } = string.Empty;

    public ICollection<ThreadModel>? Threads { get; set; } = new List<ThreadModel>();
    public ICollection<AnswerModel>? Answers { get; set; } = new List<AnswerModel>();

    public bool IsAdmin { get; set; } = false;
    public ICollection<UserFavoriteModel>? Favorites { get; set; }= new List<UserFavoriteModel>();
}