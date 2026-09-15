namespace BlixthackByMordor.ViewModels;

public class FormActionsViewModel
{
    public required string SubmitText { get; init; }
    public string CancelText { get; init; } = "Cancel";
    public string CancelController { get; init; } = "Home";
    public string CancelAction { get; init; } = "Index";
    public object? CancelRouteId { get; init; }

    public static FormActionsViewModel CancelHome(string submitText) => new()
    {
        SubmitText = submitText
    };

    public static FormActionsViewModel CancelThread(string submitText, int threadId) => new()
    {
        SubmitText = submitText,
        CancelController = "Threads",
        CancelAction = "Details",
        CancelRouteId = threadId
    };
}
