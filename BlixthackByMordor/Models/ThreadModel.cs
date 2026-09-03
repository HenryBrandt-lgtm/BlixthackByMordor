using System.ComponentModel.DataAnnotations;

namespace BlixthackByMordor.Models;

public class ThreadModel
{
    public int Id { get; set; }

    [Required, MinLength(10), MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(1), MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required] public int UserId { get; set; }
    [Required] public UserModel User { get; set; }
    [Required] public int CategoryId { get; set; }
    [Required] public CategoryModel Category { get; set; }
    [Required] public DateTime CreatedAt { get; set; }

    public ICollection<AnswerModel>? Answers { get; set; } = new List<AnswerModel>();
}