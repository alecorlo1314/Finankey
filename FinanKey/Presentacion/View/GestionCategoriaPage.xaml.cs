using FinanKey.Presentacion.ViewModels;
using Syncfusion.Maui.Toolkit.BottomSheet;

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

    private void OnEntryFocused(Border border)
    {
        border.Stroke = App.Current?.Resources["ColorAzulPricipal"] as Color;
        border.StrokeThickness = 2;
    }

    private void OnEntryUnfocused(Border border)
    {
        border.Stroke = Colors.Transparent;
    }

    private void entradaDescripcion_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderNombre);

    private void entradaDescripcion_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderNombre);

    private void Button_Clicked(object sender, EventArgs e)
    {
        bottomSheetCategoria.Show();
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        bottomSheetCategoria.State = BottomSheetState.FullExpanded;
    }
}