using TaskManager.ViewModels;

namespace TaskManager.Views;

public partial class EditTaskPage : ContentPage
{
    public EditTaskPage()
    {
        InitializeComponent();
        BindingContext = new EditTaskViewModel();
    }
}