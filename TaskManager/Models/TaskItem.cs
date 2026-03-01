using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.Models;

public class TaskItem : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _category = string.Empty;
    private DateTime _dueDate = DateTime.Now;
    private Color _categoryColor = Colors.Gray;
    private Color _dueDateBadgeColor = Colors.Gray;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set
        {
            if (SetProperty(ref _description, value))
                OnPropertyChanged(nameof(HasDescription));
        }
    }

    public string Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set
        {
            if (SetProperty(ref _dueDate, value))
                OnPropertyChanged(nameof(DueDateFormatted));
        }
    }

    public Color CategoryColor
    {
        get => _categoryColor;
        set => SetProperty(ref _categoryColor, value);
    }

    public Color DueDateBadgeColor
    {
        get => _dueDateBadgeColor;
        set => SetProperty(ref _dueDateBadgeColor, value);
    }

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);
    public string DueDateFormatted => $"Due: {DueDate:MMM dd, yyyy}";

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}