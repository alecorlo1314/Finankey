using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

public partial class ConfiguracionesPage : ContentPage
{
	public ConfiguracionesPage(ViewModelAjustes viewModelAjustes)
	{
		InitializeComponent();
		BindingContext = viewModelAjustes;
    }
}