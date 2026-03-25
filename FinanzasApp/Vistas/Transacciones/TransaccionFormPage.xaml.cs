using FinanzasApp.Presentacion.ViewModels.Transacciones;

namespace FinanzasApp.Presentacion.Vistas.Transacciones;

public partial class TransaccionFormPage : ContentPage
{
    private readonly TransaccionFormViewModel _viewModel;

    public TransaccionFormPage(TransaccionFormViewModel viewModel)
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
