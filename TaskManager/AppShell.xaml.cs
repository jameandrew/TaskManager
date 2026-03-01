using TaskManager.Views;

namespace TaskManager;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(AddTaskPage), typeof(AddTaskPage));
        Routing.RegisterRoute(nameof(EditTaskPage), typeof(EditTaskPage));
        Routing.RegisterRoute(nameof(CategoryTasksPage), typeof(CategoryTasksPage));
    }
}