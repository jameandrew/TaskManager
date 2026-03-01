using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Views;

namespace TaskManager.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly TaskService _taskService;
    private ObservableCollection<CategoryItem> _categories;
    private ObservableCollection<PendingTaskItem> _pendingTasks;
    private bool _isEmptyStateVisible;
    private bool _areSectionsVisible;

    public ObservableCollection<CategoryItem> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    public ObservableCollection<PendingTaskItem> PendingTasks
    {
        get => _pendingTasks;
        set => SetProperty(ref _pendingTasks, value);
    }

    public bool IsEmptyStateVisible
    {
        get => _isEmptyStateVisible;
        set => SetProperty(ref _isEmptyStateVisible, value);
    }

    public bool AreSectionsVisible
    {
        get => _areSectionsVisible;
        set => SetProperty(ref _areSectionsVisible, value);
    }

    public ICommand CategorySelectedCommand { get; }
    public ICommand PendingTaskSelectedCommand { get; }
    public ICommand AddTaskCommand { get; }

    public HomeViewModel()
    {
        _taskService = TaskService.Instance;
        _categories = new ObservableCollection<CategoryItem>();
        _pendingTasks = new ObservableCollection<PendingTaskItem>();

        CategorySelectedCommand = new Command<CategoryItem>(OnCategorySelected);
        PendingTaskSelectedCommand = new Command<PendingTaskItem>(OnPendingTaskSelected);
        AddTaskCommand = new Command(OnAddTask);

        _taskService.TasksChanged += OnTasksChanged;
        LoadData();
    }

    private void OnTasksChanged(object? sender, EventArgs e)
    {
        LoadData();
    }

    public void LoadData()
    {
        // Force update all category task counts
        foreach (var category in _taskService.Categories)
        {
            category.TaskCount = _taskService.GetTaskCountForCategory(category.CategoryKey);
        }

        // Clear and reload categories
        Categories.Clear();

        foreach (var category in _taskService.Categories.Where(c => c.TaskCount > 0))
        {
            Categories.Add(category);
        }

        // Refresh pending tasks
        var pendingTasks = _taskService.GetPendingTasks();
        PendingTasks.Clear();
        foreach (var task in pendingTasks)
        {
            PendingTasks.Add(task);
        }

        // Update visibility
        bool hasTasks = _taskService.AllTasks.Count > 0;
        IsEmptyStateVisible = !hasTasks;
        AreSectionsVisible = hasTasks;
    }

    private async void OnCategorySelected(CategoryItem? category)
    {
        if (category == null)
            return;

        await Shell.Current.GoToAsync(nameof(CategoryTasksPage), new Dictionary<string, object>
        {
            { "Category", category }
        });
    }

    private async void OnPendingTaskSelected(PendingTaskItem? pendingTask)
    {
        if (pendingTask?.OriginalTask == null)
            return;

        await Shell.Current.GoToAsync(nameof(EditTaskPage), new Dictionary<string, object>
        {
            { "Task", pendingTask.OriginalTask }
        });
    }

    private async void OnAddTask()
    {
        await Shell.Current.GoToAsync(nameof(AddTaskPage));
    }

    public void Cleanup()
    {
        _taskService.TasksChanged -= OnTasksChanged;
    }
}