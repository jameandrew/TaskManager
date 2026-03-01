using TaskManager.ViewModels;

namespace TaskManager.Views;

public partial class CategoryTasksPage : ContentPage
{
    private readonly CategoryTasksViewModel _viewModel;

    public CategoryTasksPage()
    {
        InitializeComponent();
        _viewModel = new CategoryTasksViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Cleanup();
    }
}