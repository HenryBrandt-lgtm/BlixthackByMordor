using BlixthackByMordor.Models;

namespace BlixthackByMordor.ViewModels;

public class HomeViewModel
{
    public List<ThreadModel> Threads { get; set; } = [];
    public List<CategoryModel> Categories { get; set; } = [];
}