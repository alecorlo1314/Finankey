using FinanKey.Presentacion.View;

namespace FinanKey;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rutas para navegación programática
        Routing.RegisterRoute("AgregarTarjetaPage", typeof(AgregarTarjetaPage));
    }
}