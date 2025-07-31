
using FinanKey.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanKey.View;

public partial class FinanzasPage : ContentPage
{
    public FinanzasPage(ViewModelFinanzas viewModelFinanzas)
	{
		InitializeComponent();
        BindingContext = viewModelFinanzas;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ViewModelFinanzas viewModel)
        {
            _ = viewModel.InicializarAsync();
        }
    }


    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        borderBarraBusqueda.Stroke = Color.Parse("#4f46e5");
        borderBarraBusqueda.StrokeThickness = 2;
    }

    private void Entry_Unfocused(object sender, FocusEventArgs e)
    {
        borderBarraBusqueda.Stroke = Colors.Transparent;
    }
}