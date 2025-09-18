using FinanKey.Dominio.Models;
using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

public partial class DetalleTarjetaPage : ContentPage
{
	public DetalleTarjetaPage(ViewModelDetalleTarjeta detalleTarjeta)
	{
		InitializeComponent();
        //Inyeccion de dependencias del ViewModelDetalleTarjeta
        BindingContext = detalleTarjeta;
    }

    /// <summary>
    /// Metodo que se ejecuta al navegar a la pagina
    /// </summary>
    /// <param name="args"></param>
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }
}