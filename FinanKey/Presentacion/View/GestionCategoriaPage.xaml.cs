using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

public partial class GestionCategoriaPage : ContentPage
{
	public GestionCategoriaPage(ViewModelGestionCategorias viewModelGestionCategorias)
	{
		InitializeComponent();
        BindingContext = viewModelGestionCategorias;
	}

    private void entradaDescripcion_Focused(object sender, FocusEventArgs e)
    {

    }

    private void entradaDescripcion_Unfocused(object sender, FocusEventArgs e)
    {

    }
}