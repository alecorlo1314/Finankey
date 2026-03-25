using FinanzasApp.Presentacion.Vistas.Tarjetas;
using FinanzasApp.Presentacion.Vistas.Transacciones;

namespace FinanzasApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegistrarRutas();
        }

        /// <summary>
        /// Registra rutas para páginas que se navegan mediante GoToAsync
        /// pero no son tabs principales del TabBar.
        /// </summary>
        private static void RegistrarRutas()
        {
            // ── Rutas de tarjetas ─────────────────────────────────────────────
            Routing.RegisterRoute("tarjetas/nueva", typeof(TarjetaFormPage));
            Routing.RegisterRoute("tarjetas/editar", typeof(TarjetaFormPage));
            Routing.RegisterRoute("tarjetas/detalle", typeof(TarjetaDetallePage));

            // ── Rutas de transacciones ────────────────────────────────────────
            Routing.RegisterRoute("transacciones/nueva", typeof(TransaccionFormPage));
            Routing.RegisterRoute("transacciones/editar", typeof(TransaccionFormPage));
        }
    }
}
