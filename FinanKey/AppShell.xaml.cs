using FinanKey.View;
namespace FinanKey
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            // Registrar rutas para la navegación
            Routing.RegisterRoute("loginPage", typeof(LoginPage));
            Routing.RegisterRoute("detalleCuentaPage", typeof(DetalleCuentaPage));
        }
    }
}
