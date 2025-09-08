using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Presentacion.View;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelAjustes : ObservableObject
    {
        //Navegacion a GestionarCategoriasPage
        [RelayCommand]
        public async Task NavegarGestionCategoriaPage()
        {
            await Shell.Current.GoToAsync(nameof(GestionCategoriaPage));
        }
    }
}
