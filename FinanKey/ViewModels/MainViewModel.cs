using FinanKey.Models;
using FinanKey.Servicios;
using System.Linq;

namespace FinanKey.Maui.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly IServicioTarjeta _servicioTarjeta;
    private readonly IServicioMovimiento _servicioMovimiento;

    public MainViewModel(IServicioTarjeta servicioTarjeta, IServicioMovimiento servicioMovimiento)
    {
        _servicioTarjeta = servicioTarjeta;
        _servicioMovimiento = servicioMovimiento;
    }

    private List<Tarjeta> _tarjetas = new();
    public List<Tarjeta> Tarjetas { get => _tarjetas; set => Set(ref _tarjetas, value); }

    private Tarjeta? _seleccionada;
    public Tarjeta? Seleccionada { get => _seleccionada; set { Set(ref _seleccionada, value); _ = LoadMovementsForSelectedAsync(); } }

    private List<Movimiento> _ultimos = new();
    public List<Movimiento> Ultimos { get => _ultimos; set => Set(ref _ultimos, value); }

    public async Task LoadAsync()
    {
        Tarjetas = await _servicioTarjeta.ObtenerTarjetasAsync();
        Seleccionada ??= Tarjetas.FirstOrDefault();
        await LoadMovementsForSelectedAsync();
    }

    private async Task LoadMovementsForSelectedAsync()
    {
        if (Seleccionada == null) { 
            Ultimos = new(); 
            return; 
        }
        var lista = await _servicioMovimiento.ObtenerMovimientoPorIdAsync(Seleccionada.Id);  
    }
}
