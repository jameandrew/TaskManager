namespace TaskManager.Models;

public class PendingTaskItem
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string DueDateText { get; set; } = string.Empty;
    public Color DueDateColor { get; set; } = Colors.Gray;
    public TaskItem? OriginalTask { get; set; }
}