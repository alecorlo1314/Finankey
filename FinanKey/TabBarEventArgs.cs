namespace FinanKey
{
    public class TabBarEventArgs : EventArgs
    {
        public AppShell.PageType CurrentPage { get; private set; }

        public TabBarEventArgs(AppShell.PageType currentPage)
        {
            CurrentPage = currentPage;
        }
    }
}
