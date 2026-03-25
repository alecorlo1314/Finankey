using FinanzasApp.Presentacion.ViewModels.Configuracion;

namespace FinanzasApp.Presentacion.Vistas.Configuracion;

public partial class ConfiguracionPage : ContentPage
{
    private readonly ConfiguracionViewModel _viewModel;

    public ConfiguracionPage(ConfiguracionViewModel viewModel)
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
