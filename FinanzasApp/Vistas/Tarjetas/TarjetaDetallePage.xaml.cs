using FinanzasApp.Presentacion.ViewModels.Transacciones;

namespace FinanzasApp.Presentacion.Vistas.Tarjetas;

/// <summary>
/// Página de detalle de una tarjeta. Reutiliza TransaccionesViewModel
/// para mostrar el historial de movimientos de esa tarjeta específica.
/// </summary>
public partial class TarjetaDetallePage : ContentPage
{
    private readonly TransaccionesViewModel _viewModel;

    public TarjetaDetallePage(TransaccionesViewModel viewModel)
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
