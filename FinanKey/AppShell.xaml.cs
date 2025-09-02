using FinanKey.View;
using SimpleToolkit.Core;
using SimpleToolkit.SimpleShell;

namespace FinanKey
{
    public partial class AppShell : SimpleShell
    {
        public AppShell()
        {
            InitializeComponent();
            ///Registro de Rutas
            Routing.RegisterRoute(nameof(AgregarTarjetaPage), typeof(AgregarTarjetaPage));

            AddTab(typeof(InicioPage), PageType.InicioPage);
            AddTab(typeof(AnadirPage), PageType.AnadirPage);
            AddTab(typeof(ReportesPage), PageType.ReportesPage);
            AddTab(typeof(ConfiguracionesPage), PageType.ConfiguracionPage);

            Loaded += AppShellLoaded;
            //Sucribir el evento OnShellNavigated
            this.Navigated += OnShellNavigated;
        }

        private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e)
        {
            // Obtén la ruta de la página actual
            var currentLocation = e.Current.Location.OriginalString;

            // Lista de páginas donde SÍ quieres mostrar el TabBar
            var mostrarTabBarEn = new[]
            {
            nameof(PageType.InicioPage),
            nameof(PageType.AnadirPage),
            nameof(PageType.ReportesPage),
            nameof(PageType.ConfiguracionPage)
        };

            // Si la ruta contiene alguna de las páginas principales, muestra el TabBar
            bool mostrar = mostrarTabBarEn.Any(p => currentLocation.Contains(p));

            tabBarView.IsVisible = mostrar;
        }

        private static void AppShellLoaded(object sender, EventArgs e)
        {
            var shell = sender as AppShell;

            shell.Window.SubscribeToSafeAreaChanges(safeArea =>
            {
                shell.paginaContenedor.Margin = safeArea;
                shell.tabBarView.TabsPadding = new Thickness(safeArea.Left, 0, safeArea.Right, safeArea.Bottom);
            });
        }

        private void AddTab(Type page, PageType pageEnum)
        {
            var tab = new Tab { Route = pageEnum.ToString(), Title = pageEnum.ToString() };
            tab.Items.Add(new ShellContent { ContentTemplate = new DataTemplate(page) });

            tabBar.Items.Add(tab);
        }
        private void TabBarViewCurrentPageChanged(object sender, TabBarEventArgs e)
        {
            Shell.Current.GoToAsync("///" + e.CurrentPage.ToString());
        }

        public enum PageType
        {
            InicioPage, AnadirPage, ReportesPage, ConfiguracionPage
        }
    }
}