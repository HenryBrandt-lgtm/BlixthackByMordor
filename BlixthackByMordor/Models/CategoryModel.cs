using System.ComponentModel.DataAnnotations;

namespace BlixthackByMordor.Models;

public class CategoryModel
{
    public int Id { get; set; }

    [Required, MinLength(1), MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required] public DateTime CreatedAt { get; set; }

    public ICollection<ThreadModel>? Threads { get; set; } = new List<ThreadModel>();
}