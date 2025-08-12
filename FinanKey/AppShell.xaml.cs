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

            AddTab(typeof(FinanzasPage), PageType.FinanzasPage);
            AddTab(typeof(AnadirPage), PageType.AnadirPage);
            AddTab(typeof(ReportesPage), PageType.ReportesPage);
            AddTab(typeof(ConfiguracionesPage), PageType.ConfiguracionPage);

            Loaded += AppShellLoaded;
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
            FinanzasPage, AnadirPage, ReportesPage, ConfiguracionPage
        }
    }
}