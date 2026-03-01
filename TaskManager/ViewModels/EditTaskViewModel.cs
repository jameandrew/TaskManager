using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ViewModels;

public class EditTaskViewModel : BaseViewModel, IQueryAttributable
{
    private readonly TaskService _taskService;
    private TaskItem? _currentTask;
    private string _taskName = string.Empty;
    private string _taskDescription = string.Empty;
    private string _selectedCategory = string.Empty;
    private DateTime _dueDate;
    private bool _isCompleted;

    public string TaskName
    {
        get => _taskName;
        set => SetProperty(ref _taskName, value);
    }

    public string TaskDescription
    {
        get => _taskDescription;
        set => SetProperty(ref _taskDescription, value);
    }

    public string SelectedCategory
    {
        get => _selectedCategory;
        set => SetProperty(ref _selectedCategory, value);
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public bool IsCompleted
    {
        get => _isCompleted;
        set => SetProperty(ref _isCompleted, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand CancelCommand { get; }

    public EditTaskViewModel()
    {
        _taskService = TaskService.Instance;

        SaveCommand = new Command(async () => await OnSave());
        DeleteCommand = new Command(async () => await OnDelete());
        CancelCommand = new Command(async () => await OnCancel());
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Task", out var task) && task is TaskItem taskItem)
        {
            _currentTask = taskItem;
            TaskName = taskItem.Name;
            TaskDescription = taskItem.Description;
            SelectedCategory = taskItem.Category;
            DueDate = taskItem.DueDate;
        }
    }

    private async Task OnSave()
    {
        if (_currentTask == null)
            return;

        if (string.IsNullOrWhiteSpace(TaskName))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a task name", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory))
        {
            await Shell.Current.DisplayAlert("Error", "Please select a category", "OK");
            return;
        }

        _currentTask.Name = TaskName;
        _currentTask.Description = TaskDescription;
        _currentTask.Category = SelectedCategory;
        _currentTask.DueDate = DueDate;
        _currentTask.CategoryColor = _taskService.GetCategoryColor(SelectedCategory);

        if (IsCompleted)
        {
            _taskService.RemoveTask(_currentTask);
            await Shell.Current.DisplayAlert("Success", "Task completed and removed!", "OK");
        }
        else
        {
            _taskService.UpdateTask();
            await Shell.Current.DisplayAlert("Success", "Task updated successfully!", "OK");
        }

        await Shell.Current.GoToAsync("..");
    }

    private async Task OnDelete()
    {
        if (_currentTask == null)
            return;

        bool confirm = await Shell.Current.DisplayAlert("Delete Task",
            $"Are you sure you want to delete '{TaskName}'?", "Yes", "No");

        if (confirm)
        {
            _taskService.RemoveTask(_currentTask);
            await Shell.Current.DisplayAlert("Success", "Task deleted!", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }

    private async Task OnCancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}