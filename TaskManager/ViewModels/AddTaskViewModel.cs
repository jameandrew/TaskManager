using System.Collections.ObjectModel;
using System.Windows.Input;
using TaskManager.Models;
using TaskManager.Services;

namespace TaskManager.ViewModels;

public class AddTaskViewModel : BaseViewModel
{
    private readonly TaskService _taskService;
    private string _taskName = string.Empty;
    private string _taskDescription = string.Empty;
    private string _selectedCategory = string.Empty;
    private DateTime _dueDate;
    private bool _isAddTaskVisible;
    private bool _isCustomCategoryVisible;
    private string _customCategoryName = string.Empty;
    private ObservableCollection<string> _availableCategories;

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
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                OnCategoryChanged();
            }
        }
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set => SetProperty(ref _dueDate, value);
    }

    public bool IsAddTaskVisible
    {
        get => _isAddTaskVisible;
        set => SetProperty(ref _isAddTaskVisible, value);
    }

    public bool IsCustomCategoryVisible
    {
        get => _isCustomCategoryVisible;
        set => SetProperty(ref _isCustomCategoryVisible, value);
    }

    public string CustomCategoryName
    {
        get => _customCategoryName;
        set => SetProperty(ref _customCategoryName, value);
    }

    public ObservableCollection<string> AvailableCategories
    {
        get => _availableCategories;
        set => SetProperty(ref _availableCategories, value);
    }

    public ICommand AddTaskCommand { get; }
    public ICommand AddCustomCategoryCommand { get; }

    public AddTaskViewModel()
    {
        _taskService = TaskService.Instance;
        DueDate = DateTime.Now;
        _availableCategories = new ObservableCollection<string>();

        AddTaskCommand = new Command(async () => await OnAddTask());
        AddCustomCategoryCommand = new Command(OnAddCustomCategory);

        LoadCategories();
    }

    private void LoadCategories()
    {
        AvailableCategories.Clear();

        // Add predefined categories
        AvailableCategories.Add("Work");
        AvailableCategories.Add("Personal");
        AvailableCategories.Add("Shopping");
        AvailableCategories.Add("Health");
        AvailableCategories.Add("Education");
        AvailableCategories.Add("Programming");
        AvailableCategories.Add("Learning");
        AvailableCategories.Add("Finance");
        AvailableCategories.Add("Home");
        AvailableCategories.Add("Travel");
        AvailableCategories.Add("Food");
        AvailableCategories.Add("Fitness");
        AvailableCategories.Add("Hobby");
        AvailableCategories.Add("Other");

        // Add custom categories from existing tasks
        foreach (var category in _taskService.Categories)
        {
            if (!AvailableCategories.Contains(category.CategoryKey))
            {
                AvailableCategories.Add(category.CategoryKey);
            }
        }

        AvailableCategories.Add("+ Add Custom Category");
    }

    private void OnCategoryChanged()
    {
        if (SelectedCategory == "+ Add Custom Category")
        {
            IsCustomCategoryVisible = true;
            IsAddTaskVisible = false;
        }
        else if (!string.IsNullOrWhiteSpace(SelectedCategory))
        {
            IsCustomCategoryVisible = false;
            IsAddTaskVisible = true;
        }
    }

    private void OnAddCustomCategory()
    {
        if (!string.IsNullOrWhiteSpace(CustomCategoryName))
        {
            var newCategory = CustomCategoryName.Trim();

            // Add to available categories list (before the "+ Add Custom Category" option)
            if (!AvailableCategories.Contains(newCategory))
            {
                AvailableCategories.Insert(AvailableCategories.Count - 1, newCategory);
            }

            SelectedCategory = newCategory;
            CustomCategoryName = string.Empty;
            IsCustomCategoryVisible = false;
            IsAddTaskVisible = true;
        }
    }

    private async Task OnAddTask()
    {
        if (string.IsNullOrWhiteSpace(TaskName))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter a task name", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedCategory) || SelectedCategory == "+ Add Custom Category")
        {
            await Shell.Current.DisplayAlert("Error", "Please select a valid category", "OK");
            return;
        }

        var newTask = new TaskItem
        {
            Name = TaskName,
            Description = TaskDescription,
            Category = SelectedCategory,
            DueDate = DueDate,
            CategoryColor = _taskService.GetCategoryColor(SelectedCategory)
        };

        _taskService.AddTask(newTask);

        TaskName = string.Empty;
        TaskDescription = string.Empty;
        DueDate = DateTime.Now;
        SelectedCategory = string.Empty;
        IsAddTaskVisible = false;

        // Reload categories to include any newly created ones
        LoadCategories();

        await Shell.Current.DisplayAlert("Success", "Task added successfully!", "OK");
    }
}