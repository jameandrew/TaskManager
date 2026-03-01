using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Services;
using TaskManager.Views;

namespace TaskManager.ViewModels;

public class CategoryTasksViewModel : BaseViewModel, IQueryAttributable
{
    private readonly TaskService _taskService;
    private CategoryItem? _category;
    private ObservableCollection<TaskItem> _categoryTasks;
    private string _categoryName = string.Empty;
    private string _categoryIcon = string.Empty;
    private Color _categoryIconBackgroundColor;
    private string _categoryTaskCount = string.Empty;

    public ObservableCollection<TaskItem> CategoryTasks
    {
        get => _categoryTasks;
        set => SetProperty(ref _categoryTasks, value);
    }

    public string CategoryName
    {
        get => _categoryName;
        set => SetProperty(ref _categoryName, value);
    }

    public string CategoryIcon
    {
        get => _categoryIcon;
        set => SetProperty(ref _categoryIcon, value);
    }

    public Color CategoryIconBackgroundColor
    {
        get => _categoryIconBackgroundColor;
        set => SetProperty(ref _categoryIconBackgroundColor, value);
    }

    public string CategoryTaskCount
    {
        get => _categoryTaskCount;
        set => SetProperty(ref _categoryTaskCount, value);
    }

    public ICommand TaskSelectedCommand { get; }
    public ICommand AddTaskCommand { get; }

    public CategoryTasksViewModel()
    {
        _taskService = TaskService.Instance;
        _categoryTasks = new ObservableCollection<TaskItem>();
        _categoryIconBackgroundColor = Colors.Gray;

        TaskSelectedCommand = new Command<TaskItem>(OnTaskSelected);
        AddTaskCommand = new Command(OnAddTask);

        _taskService.TasksChanged += OnTasksChanged;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Category", out var category) && category is CategoryItem categoryItem)
        {
            _category = categoryItem;
            LoadCategoryData();
        }
    }

    private void OnTasksChanged(object? sender, EventArgs e)
    {
        LoadCategoryData();
    }

    public void LoadCategoryData()
    {
        if (_category == null)
            return;

        CategoryName = _category.Name;
        CategoryIcon = _category.Icon;
        CategoryIconBackgroundColor = _category.IconBackgroundColor;

        CategoryTasks.Clear();
        foreach (var task in _taskService.AllTasks)
        {
            if (task.Category.Contains(_category.CategoryKey, StringComparison.OrdinalIgnoreCase) ||
                _category.Name.Contains(task.Category, StringComparison.OrdinalIgnoreCase))
            {
                var daysDiff = (task.DueDate - DateTime.Now).Days;
                if (daysDiff < 0)
                    task.DueDateBadgeColor = Color.FromArgb("#DC3545");
                else if (daysDiff == 0)
                    task.DueDateBadgeColor = Color.FromArgb("#DC3545");
                else if (daysDiff == 1)
                    task.DueDateBadgeColor = Color.FromArgb("#FF9500");
                else if (daysDiff <= 7)
                    task.DueDateBadgeColor = Color.FromArgb("#FFC107");
                else
                    task.DueDateBadgeColor = Color.FromArgb("#28A745");

                CategoryTasks.Add(task);
            }
        }

        CategoryTaskCount = $"{CategoryTasks.Count} {(CategoryTasks.Count == 1 ? "Task" : "Tasks")}";
    }

    private async void OnTaskSelected(TaskItem? task)
    {
        if (task == null)
            return;

        await Shell.Current.GoToAsync(nameof(EditTaskPage), new Dictionary<string, object>
        {
            { "Task", task }
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