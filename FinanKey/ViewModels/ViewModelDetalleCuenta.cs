using CommunityToolkit.Mvvm.ComponentModel;
using FinanKey.Models;

namespace FinanKey.ViewModels
{
    [QueryProperty(nameof(Cuenta), nameof(Cuenta))]
    public partial class ViewModelDetalleCuenta : ObservableObject
    {
        [ObservableProperty]
         Cuenta cuenta;
    }
}
