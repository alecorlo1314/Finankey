using FinanzasApp.Presentacion.ViewModels.Tarjetas;

namespace FinanzasApp.Presentacion.Vistas.Tarjetas;

/// <summary>
/// Code-behind de la lista de tarjetas.
/// Toda la lógica está en TarjetasViewModel.
/// </summary>
public partial class TarjetasPage : ContentPage
{
    private readonly TarjetasViewModel _viewModel;

    public TarjetasPage(TarjetasViewModel viewModel)
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

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        //await _viewModel.TerminarAsync();
    }
}
