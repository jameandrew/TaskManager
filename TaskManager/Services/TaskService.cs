using System.Collections.ObjectModel;
using TaskManager.Models;

namespace TaskManager.Services;

public class TaskService
{
    private static TaskService? _instance;
    public static TaskService Instance => _instance ??= new TaskService();

    public ObservableCollection<TaskItem> AllTasks { get; }
    public ObservableCollection<CategoryItem> Categories { get; }

    public event EventHandler? TasksChanged;

    private TaskService()
    {
        AllTasks = new ObservableCollection<TaskItem>();
        Categories = new ObservableCollection<CategoryItem>();
        InitializeCategories();
    }

    private void InitializeCategories()
    {
        Categories.Add(new CategoryItem
        {
            Name = "Programming Task",
            Description = "Development tasks and learning progress",
            Icon = "💻",
            IconBackgroundColor = Color.FromArgb("#5856D6"),
            CategoryKey = "Programming"
        });
        Categories.Add(new CategoryItem
        {
            Name = "Learning Task",
            Description = "Educational content and productivity",
            Icon = "📚",
            IconBackgroundColor = Color.FromArgb("#FF9500"),
            CategoryKey = "Learning"
        });
    }

    public void AddTask(TaskItem task)
    {
        AllTasks.Add(task);
        EnsureCategoryExists(task.Category);
        UpdateAllCategoryTaskCounts();
        TasksChanged?.Invoke(this, EventArgs.Empty);
    }

    public void RemoveTask(TaskItem task)
    {
        AllTasks.Remove(task);
        UpdateAllCategoryTaskCounts();
        TasksChanged?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateTask()
    {
        UpdateAllCategoryTaskCounts();
        TasksChanged?.Invoke(this, EventArgs.Empty);
    }

    private void EnsureCategoryExists(string categoryName)
    {
        var existingCategory = Categories.FirstOrDefault(c =>
            c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase) ||
            c.CategoryKey.Equals(categoryName, StringComparison.OrdinalIgnoreCase) ||
            c.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase) ||
            categoryName.Contains(c.CategoryKey, StringComparison.OrdinalIgnoreCase));

        if (existingCategory == null)
        {
            var newCategory = new CategoryItem
            {
                Name = $"{categoryName}",
                Description = $"Tasks related to {categoryName.ToLower()}",
                Icon = GetIconForCategory(categoryName),
                IconBackgroundColor = GetRandomColor(),
                CategoryKey = categoryName,
                TaskCount = 0
            };
            Categories.Add(newCategory);
        }
    }

    private string GetIconForCategory(string categoryName)
    {
        return categoryName.ToLower() switch
        {
            var c when c.Contains("work") => "💼",
            var c when c.Contains("personal") => "👤",
            var c when c.Contains("shopping") => "🛒",
            var c when c.Contains("health") => "❤️",
            var c when c.Contains("education") || c.Contains("learning") => "📚",
            var c when c.Contains("programming") || c.Contains("code") => "💻",
            var c when c.Contains("finance") || c.Contains("money") => "💰",
            var c when c.Contains("home") || c.Contains("house") => "🏠",
            var c when c.Contains("travel") => "✈️",
            var c when c.Contains("food") || c.Contains("cooking") => "🍳",
            var c when c.Contains("fitness") || c.Contains("gym") => "💪",
            var c when c.Contains("hobby") => "🎨",
            _ => "📋"
        };
    }

    private Color GetRandomColor()
    {
        var colors = new[]
        {
            "#007AFF", "#5856D6", "#FF9500", "#4CD964",
            "#FF2D55", "#5AC8FA", "#FFCC00", "#FF3B30"
        };
        var random = new Random();
        return Color.FromArgb(colors[random.Next(colors.Length)]);
    }

    private void UpdateAllCategoryTaskCounts()
    {
        foreach (var category in Categories)
        {
            category.TaskCount = GetTaskCountForCategory(category.CategoryKey);
        }
    }

    public int GetTaskCountForCategory(string categoryKey)
    {
        return AllTasks.Count(t =>
            t.Category.Equals(categoryKey, StringComparison.OrdinalIgnoreCase) ||
            t.Category.Contains(categoryKey, StringComparison.OrdinalIgnoreCase) ||
            categoryKey.Contains(t.Category, StringComparison.OrdinalIgnoreCase));
    }

    public ObservableCollection<PendingTaskItem> GetPendingTasks()
    {
        var pendingTasks = new ObservableCollection<PendingTaskItem>();

        foreach (var task in AllTasks)
        {
            var daysDiff = (task.DueDate - DateTime.Now).Days;
            string dueDateText;
            Color dueDateColor;

            if (daysDiff < 0)
            {
                dueDateText = "Overdue";
                dueDateColor = Color.FromArgb("#DC3545");
            }
            else if (daysDiff == 0)
            {
                dueDateText = "Today";
                dueDateColor = Color.FromArgb("#DC3545");
            }
            else if (daysDiff == 1)
            {
                dueDateText = "Tomorrow";
                dueDateColor = Color.FromArgb("#FF9500");
            }
            else if (daysDiff <= 7)
            {
                dueDateText = $"{daysDiff} days";
                dueDateColor = Color.FromArgb("#FFC107");
            }
            else
            {
                dueDateText = task.DueDate.ToString("MMM dd");
                dueDateColor = Color.FromArgb("#28A745");
            }

            // Set badge color on task
            task.DueDateBadgeColor = dueDateColor;

            pendingTasks.Add(new PendingTaskItem
            {
                Name = task.Name,
                Category = task.Category,
                DueDateText = dueDateText,
                DueDateColor = dueDateColor,
                OriginalTask = task
            });
        }

        return pendingTasks;
    }

    public Color GetCategoryColor(string category)
    {
        return category.ToLower() switch
        {
            var c when c.Contains("work") => Color.FromArgb("#007AFF"),
            var c when c.Contains("personal") => Color.FromArgb("#5856D6"),
            var c when c.Contains("shopping") => Color.FromArgb("#FF9500"),
            var c when c.Contains("health") => Color.FromArgb("#4CD964"),
            var c when c.Contains("education") || c.Contains("learning") => Color.FromArgb("#FF9500"),
            var c when c.Contains("programming") => Color.FromArgb("#5856D6"),
            _ => Color.FromArgb("#8E8E93")
        };
    }
}