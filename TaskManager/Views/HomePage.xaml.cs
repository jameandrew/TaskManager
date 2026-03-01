using TaskManager.ViewModels;

namespace TaskManager.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage()
    {
        InitializeComponent();
        _viewModel = new HomeViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Force reload data when page appears
        _viewModel.LoadData();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Don't cleanup here - we want to keep listening to changes
    }
}