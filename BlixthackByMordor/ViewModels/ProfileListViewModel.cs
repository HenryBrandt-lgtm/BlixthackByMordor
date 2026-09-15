namespace BlixthackByMordor.ViewModels;

public class ProfileListViewModel
{
    public required string HeadingId { get; init; }
    public required string Title { get; init; }
    public required string EmptyText { get; init; }
    public string? CountId { get; init; }
    public required IReadOnlyList<ActivityRowViewModel> Items { get; init; }

    public int Count => Items.Count;

    public static ProfileListViewModel Threads(IReadOnlyList<ProfileThreadItem> threads) => new()
    {
        HeadingId = "profile-threads-heading",
        Title = "Threads",
        EmptyText = "No threads yet.",
        Items = threads.Select(ActivityRowViewModel.FromThread).ToList()
    };

    public static ProfileListViewModel Answers(IReadOnlyList<ProfileAnswerItem> answers) => new()
    {
        HeadingId = "profile-answers-heading",
        Title = "Answers",
        EmptyText = "No answers yet.",
        Items = answers.Select(ActivityRowViewModel.FromAnswer).ToList()
    };

    public static ProfileListViewModel Favorites(IReadOnlyList<ProfileFavoriteAnswerItem> favorites) => new()
    {
        HeadingId = "profile-favorites-heading",
        Title = "Favorite answers",
        EmptyText = "No answers yet.",
        CountId = "favorite-count",
        Items = favorites.Select(ActivityRowViewModel.FromFavorite).ToList()
    };
}

public class ActivityRowViewModel
{
    public int ThreadId { get; init; }
    public required string Title { get; init; }
    public required string Byline { get; init; }
    public DateTime CreatedAt { get; init; }
    public int? FavoriteAnswerId { get; init; }

    public static ActivityRowViewModel FromThread(ProfileThreadItem thread) => new()
    {
        ThreadId = thread.Id,
        Title = thread.Title,
        Byline = thread.CategoryName,
        CreatedAt = thread.CreatedAt
    };

    public static ActivityRowViewModel FromAnswer(ProfileAnswerItem answer) => new()
    {
        ThreadId = answer.ThreadId,
        Title = answer.Excerpt,
        Byline = $"on {answer.ThreadTitle}",
        CreatedAt = answer.CreatedAt
    };

    public static ActivityRowViewModel FromFavorite(ProfileFavoriteAnswerItem answer) => new()
    {
        ThreadId = answer.ThreadId,
        Title = answer.Excerpt,
        Byline = $"on {answer.ThreadTitle}",
        CreatedAt = answer.CreatedAt,
        FavoriteAnswerId = answer.AnswerId
    };
}
