namespace BlixthackByMordor.Helpers;

public static class DisplayHelpers
{
    public static string FormatAge(DateTime createdAt)
    {
        var local = createdAt.ToLocalTime();
        var elapsed = DateTime.Now - local;
        if (elapsed.TotalMinutes < 1) return "Just now";
        if (elapsed.TotalHours < 1) return $"{(int)elapsed.TotalMinutes}m ago";
        if (elapsed.TotalDays < 1) return $"{(int)elapsed.TotalHours}h ago";
        if (elapsed.TotalDays < 7) return $"{(int)elapsed.TotalDays}d ago";
        return local.ToString("MMM d");
    }

    public static string Initial(string? username) =>
        string.IsNullOrEmpty(username) ? "?" : username[0].ToString().ToUpperInvariant();
}
