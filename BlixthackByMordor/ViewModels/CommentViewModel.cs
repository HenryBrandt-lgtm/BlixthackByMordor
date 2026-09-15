using BlixthackByMordor.Models;

namespace BlixthackByMordor.ViewModels;

public class CommentViewModel
{
    public required AnswerModel Answer { get; init; }
    public IReadOnlyList<AnswerModel> Nested { get; init; } = [];
    public bool IsNested { get; init; }
    public bool CanReply { get; init; }
    public int CurrentUserId { get; init; }
    public bool IsAdmin { get; init; }

    public bool CanDelete =>
        CurrentUserId > 0 && (CurrentUserId == Answer.UserId || IsAdmin);

    public bool IsFavorited =>
        Answer.Favorites?.Any(favorite => favorite.UserId == CurrentUserId) == true;

    public static CommentViewModel FromAnswer(
        AnswerModel answer,
        IReadOnlyList<AnswerModel> nested,
        bool canReply,
        int currentUserId,
        bool isAdmin) => new()
    {
        Answer = answer,
        Nested = nested,
        CanReply = canReply,
        CurrentUserId = currentUserId,
        IsAdmin = isAdmin
    };

    public CommentViewModel ForNested(AnswerModel reply) => new()
    {
        Answer = reply,
        IsNested = true,
        CurrentUserId = CurrentUserId,
        IsAdmin = IsAdmin
    };
}
