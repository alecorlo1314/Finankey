using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FinanKey.Maui.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return;
        field = value!;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}