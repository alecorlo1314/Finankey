using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Aplicacion.Transacciones.Comandos;
using FinanzasApp.Aplicacion.Transacciones.Consultas;
using FinanzasApp.Domain.Enumeraciones;
using System.Collections.ObjectModel;

namespace FinanzasApp.Presentacion.ViewModels.Transacciones;

/// <summary>
/// ViewModel del tab global de Movimientos.
/// Muestra TODAS las transacciones de TODAS las tarjetas combinadas.
/// Es independiente de TransaccionesViewModel que trabaja por tarjeta específica.
/// </summary>
public partial class MovimientosViewModel(IMediator mediador) : ViewModelBase
{
    #region 🧾 Estado de la UI

    // Colección agrupada para el CollectionView con IsGrouped=True
    [ObservableProperty]
    private ObservableCollection<GrupoTransacciones> _transaccionesAgrupadas = [];

    // Mantén la lista plana también para los totales
    [ObservableProperty]
    private ObservableCollection<TransaccionResumenDto> _transacciones = [];

    [ObservableProperty] private decimal _totalGastos;
    [ObservableProperty] private decimal _totalIngresos;
    [ObservableProperty] private decimal _balance;
    [ObservableProperty] private bool _sinTransacciones;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarLimpiarBusqueda))]
    private string _textoBusqueda = string.Empty;

    public bool MostrarLimpiarBusqueda => !string.IsNullOrWhiteSpace(TextoBusqueda);

    #endregion

    #region 🎯 Filtros

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarTabTodos))]
    [NotifyPropertyChangedFor(nameof(MostrarTabGastos))]
    [NotifyPropertyChangedFor(nameof(MostrarTabIngresos))]
    private TipoTransaccion? _filtroActivo = null;

    public bool MostrarTabTodos => FiltroActivo is null;
    public bool MostrarTabGastos => FiltroActivo == TipoTransaccion.Gasto;
    public bool MostrarTabIngresos => FiltroActivo == TipoTransaccion.Ingreso;

    #endregion

    #region 🗑️ Eliminación (BottomSheet)

    [ObservableProperty]
    private TransaccionResumenDto? _transaccionAEliminar;

    [ObservableProperty]
    private bool _mostrarSheetEliminar = false;

    [RelayCommand]
    private void MostrarConfirmacionEliminar(TransaccionResumenDto transaccion)
    {
        TransaccionAEliminar = transaccion;
        MostrarSheetEliminar = true;
    }

    [RelayCommand]
    private async Task ConfirmarEliminarAsync()
    {
        MostrarSheetEliminar = false;

        var transaccion = TransaccionAEliminar;
        TransaccionAEliminar = null;

        if (transaccion is null) return;

        await EjecutarConCargaAsync(async () =>
        {
            await mediador.EnviarAsync(new EliminarTransaccionComando(transaccion.Id));
            await CargarTransaccionesAsync();
        });
    }

    [RelayCommand]
    private void CancelarEliminar()
    {
        MostrarSheetEliminar = false;
        TransaccionAEliminar = null;
    }

    #endregion

    #region 🔄 Ciclo de vida

    public override async Task AlAparecerAsync()
    {
        Titulo = "Movimientos";
        MostrarSheetEliminar = false;
        TransaccionAEliminar = null;
        await CargarTransaccionesAsync();
    }

    #endregion

    #region 📦 Carga de datos

    // Lista completa en memoria para filtrar sin ir a la BD
    private List<TransaccionResumenDto> _todasLasTransacciones = [];

    /// <summary>
    /// Carga TODAS las transacciones de TODAS las tarjetas.
    /// Usa ObtenerTransaccionesRecientesConsulta con cantidad alta
    /// para traer el historial completo.
    /// </summary>
    [RelayCommand]
    private async Task CargarTransaccionesAsync()
    {
        await EjecutarConCargaAsync(async () =>
        {
            // Trae todas las transacciones recientes de todas las tarjetas
            // El 9999 asegura que trae el historial completo
            var consulta = new ObtenerTransaccionesRecientesConsulta(9999);
            var resultado = (await mediador.ConsultarAsync(consulta)).ToList();

            // Guarda la lista completa en memoria para el filtrado local
            _todasLasTransacciones = resultado;

            AplicarFiltros();
        });
    }
    /// <summary>
    /// Aplica filtros en memoria y agrupa por fecha.
    /// El orden de las etiquetas es:
    /// Hoy → Ayer → Esta semana → Este mes → meses anteriores por nombre
    /// </summary>
    private void AplicarFiltros()
    {
        var filtradas = _todasLasTransacciones.AsEnumerable();

        // Filtro por tipo
        if (FiltroActivo.HasValue)
            filtradas = filtradas.Where(t => t.Tipo == FiltroActivo.Value);

        // Filtro por búsqueda
        if (!string.IsNullOrWhiteSpace(TextoBusqueda))
        {
            var texto = TextoBusqueda.Trim().ToLowerInvariant();
            filtradas = filtradas.Where(t =>
                t.Descripcion.ToLowerInvariant().Contains(texto) ||
                t.NombreTarjeta.ToLowerInvariant().Contains(texto));
        }

        // Orden descendente por fecha
        var lista = filtradas
            .OrderByDescending(t => t.Fecha)
            .ToList();

        // Lista plana para totales
        Transacciones = new ObservableCollection<TransaccionResumenDto>(lista);
        SinTransacciones = !lista.Any();

        TotalGastos = lista.Where(t => t.Tipo == TipoTransaccion.Gasto).Sum(t => t.Monto);
        TotalIngresos = lista.Where(t => t.Tipo == TipoTransaccion.Ingreso).Sum(t => t.Monto);
        Balance = TotalIngresos - TotalGastos;

        // Agrupación por fecha
        TransaccionesAgrupadas = GenerarGrupos(lista);
    }

    /// <summary>
    /// Agrupa la lista de transacciones en secciones por fecha.
    /// </summary>
    private static ObservableCollection<GrupoTransacciones> GenerarGrupos(
        List<TransaccionResumenDto> lista)
    {
        var hoy = DateTime.Today;
        var ayer = hoy.AddDays(-1);
        var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);  // Domingo anterior
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

        var grupos = new ObservableCollection<GrupoTransacciones>();

        // Agrupa todas las transacciones por su etiqueta calculada
        var porEtiqueta = lista
            .GroupBy(t => ObtenerEtiqueta(t.Fecha, hoy, ayer, inicioSemana, inicioMes))
            .OrderBy(g => ObtenerOrdenGrupo(g.Key, hoy));

        foreach (var grupo in porEtiqueta)
        {
            var items = grupo.ToList();
            var subtitulo = GenerarSubtitulo(items);

            grupos.Add(new GrupoTransacciones(grupo.Key, subtitulo, items));
        }

        return grupos;
    }

    /// <summary>
    /// Determina la etiqueta del grupo según qué tan reciente es la fecha.
    /// </summary>
    private static string ObtenerEtiqueta(
        DateTime fecha,
        DateTime hoy,
        DateTime ayer,
        DateTime inicioSemana,
        DateTime inicioMes)
    {
        var soloFecha = fecha.Date;

        if (soloFecha == hoy)
            return "Hoy";

        if (soloFecha == ayer)
            return "Ayer";

        if (soloFecha >= inicioSemana)
            return "Esta semana";

        if (soloFecha >= inicioMes)
            return "Este mes";

        // Meses anteriores: nombre del mes + año
        // Ejemplo: "Febrero 2026", "Enero 2026"
        return fecha.ToString("MMMM yyyy",
            System.Globalization.CultureInfo.GetCultureInfo("es-CR"));
    }

    /// <summary>
    /// Define el orden numérico de los grupos para que
    /// "Hoy" siempre aparezca primero y los meses más
    /// recientes antes que los antiguos.
    /// </summary>
    private static int ObtenerOrdenGrupo(string etiqueta, DateTime hoy)
    {
        return etiqueta switch
        {
            "Hoy" => 0,
            "Ayer" => 1,
            "Esta semana" => 2,
            "Este mes" => 3,
            _ => 4  // Meses anteriores — se ordenan por fecha descendente arriba
        };
    }

    /// <summary>
    /// Genera el subtítulo del grupo con cantidad y total.
    /// Ejemplo: "3 movimientos · ₡45,200"
    /// </summary>
    private static string GenerarSubtitulo(List<TransaccionResumenDto> items)
    {
        var cantidad = items.Count;
        var label = cantidad == 1 ? "movimiento" : "movimientos";
        var total = items
            .Where(t => t.Tipo == TipoTransaccion.Gasto)
            .Sum(t => t.Monto);

        return total > 0
            ? $"{cantidad} {label} · ₡{total:N0} en gastos"
            : $"{cantidad} {label}";
    }

    #endregion

    #region 🎛️ Filtros y búsqueda

    [RelayCommand]
    private void CambiarFiltro(string filtro)
    {
        FiltroActivo = filtro switch
        {
            "gastos" => TipoTransaccion.Gasto,
            "ingresos" => TipoTransaccion.Ingreso,
            _ => null
        };

        // Filtra en memoria, no recarga desde BD
        AplicarFiltros();
    }

    /// <summary>
    /// Se llama desde el TextChanged del buscador.
    /// Filtra en memoria cada vez que el usuario escribe.
    /// </summary>
    [RelayCommand]
    private void BuscarTransacciones(string texto)
    {
        TextoBusqueda = texto;
        AplicarFiltros();
    }

    [RelayCommand]
    private void LimpiarBusqueda()
    {
        TextoBusqueda = string.Empty;
        AplicarFiltros();
    }

    #endregion

    #region 🧭 Navegación

    /// <summary>
    /// Desde el tab global no sabemos a qué tarjeta agregar,
    /// entonces navegamos a selección de tarjeta primero.
    /// </summary>
    [RelayCommand]
    private async Task EditarTransaccionAsync(TransaccionResumenDto transaccion) =>
        await Shell.Current.GoToAsync(
            "transacciones/editar",
            new Dictionary<string, object>
            {
                ["TransaccionId"] = transaccion.Id,
                ["TarjetaId"] = transaccion.TarjetaId,
                ["Tipo"] = transaccion.Tipo
            });

    #endregion
}