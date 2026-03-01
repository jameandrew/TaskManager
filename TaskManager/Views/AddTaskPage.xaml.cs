using TaskManager.ViewModels;

namespace TaskManager.Views;

public partial class AddTaskPage : ContentPage
{
    public AddTaskPage()
    {
        InitializeComponent();
        BindingContext = new AddTaskViewModel();
    }
}