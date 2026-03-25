using FinanzasApp.Presentacion.ViewModels;

namespace FinanzasApp.Presentacion.Vistas;

/// <summary>
/// Code-behind del Dashboard. Delega toda la lógica al ViewModel.
/// El ciclo de vida se gestiona llamando al VM desde los eventos de la página.
/// </summary>
public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.AlAparecerAsync();
    }
}
