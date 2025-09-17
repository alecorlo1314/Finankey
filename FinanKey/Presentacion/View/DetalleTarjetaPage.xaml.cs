using FinanKey.Dominio.Models;

namespace FinanKey.Presentacion.View;

[QueryProperty(nameof(Tarjeta), "id")]
public partial class DetalleTarjetaPage : ContentPage, IQueryAttributable
{
	public DetalleTarjetaPage()
	{
		InitializeComponent();
	}
}