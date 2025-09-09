using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

public partial class GestionCategoriaPage : ContentPage
{
    //Inteccion de dependencias
    private readonly ViewModelGestionCategorias _viewModelGestionCategorias;

    public GestionCategoriaPage(ViewModelGestionCategorias viewModelGestionCategorias)
	{
		InitializeComponent();
        _viewModelGestionCategorias = viewModelGestionCategorias;
        BindingContext = _viewModelGestionCategorias;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModelGestionCategorias.CargarDatosInicialesAsync();
    }

    private void entradaDescripcion_Focused(object sender, FocusEventArgs e)
    {

    }

    private void entradaDescripcion_Unfocused(object sender, FocusEventArgs e)
    {

    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        bottomSheetCategoria.Show();
    }
}