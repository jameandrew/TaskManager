using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskManager.Models;

public class CategoryItem : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _icon = string.Empty;
    private Color _iconBackgroundColor = Colors.Gray;
    private string _categoryKey = string.Empty;
    private int _taskCount;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string Icon
    {
        get => _icon;
        set => SetProperty(ref _icon, value);
    }

    public Color IconBackgroundColor
    {
        get => _iconBackgroundColor;
        set => SetProperty(ref _iconBackgroundColor, value);
    }

    public string CategoryKey
    {
        get => _categoryKey;
        set => SetProperty(ref _categoryKey, value);
    }

    public int TaskCount
    {
        get => _taskCount;
        set
        {
            if (SetProperty(ref _taskCount, value))
            {
                OnPropertyChanged(nameof(TaskCountText));
                OnPropertyChanged(nameof(HasTasks));
            }
        }
    }

    public string TaskCountText => $"{TaskCount} Tasks";
    public bool HasTasks => TaskCount > 0;

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