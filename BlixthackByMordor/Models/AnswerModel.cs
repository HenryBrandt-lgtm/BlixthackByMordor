using System.ComponentModel.DataAnnotations;

namespace BlixthackByMordor.Models;

public class AnswerModel
{
    public int Id { get; set; }

    [Required, MinLength(1), MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required] public int ThreadId { get; set; }
    public ThreadModel? Thread { get; set; }
    [Required] public int UserId { get; set; }
    [Required] public UserModel User { get; set; }
    [Required] public DateTime CreatedAt { get; set; }
    public ICollection<UserFavoriteModel>? Favorites { get; set; } = new List<UserFavoriteModel>();


    public int? ReplyId { get; set; }
}