using FinanKey.Presentacion.View;
using Microsoft.Win32;

namespace FinanKey;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rutas para navegación programática
        Routing.RegisterRoute(nameof(AgregarTarjetaPage), typeof(AgregarTarjetaPage));
        Routing.RegisterRoute(nameof(GestionCategoriaPage), typeof(GestionCategoriaPage));
    }
}