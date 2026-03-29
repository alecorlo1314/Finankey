using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Aplicacion.Tarjetas.Consultas;
using FinanzasApp.Aplicacion.Transacciones.Comandos;
using FinanzasApp.Aplicacion.Transacciones.Consultas;
using FinanzasApp.Domain.Enumeraciones;
using System.Collections.ObjectModel;

namespace FinanzasApp.Presentacion.ViewModels.Transacciones;

public partial class TransaccionesViewModel(IMediator mediador) : ViewModelBase, IQueryAttributable
{
    #region 🔧 Parámetros de navegación

    [ObservableProperty] private int _tarjetaId;
    [ObservableProperty] private string? _nombreTarjeta;
    [ObservableProperty] private TipoTarjeta _tipoTarjetaParam;

    #endregion

    #region 🧾 Estado de la UI

    // Lista de transacciones
    [ObservableProperty]
    private ObservableCollection<TransaccionResumenDto> _transacciones = [];

    // Totales
    [ObservableProperty] private decimal _totalGastos;
    [ObservableProperty] private decimal _totalIngresos;
    [ObservableProperty] private decimal _balance;

    // Propiedad para el resumen de corte
    [ObservableProperty]
    private ResumenCorteDto? _resumenCorte;

    // Estado vacío
    [ObservableProperty] private bool _sinTransacciones;

    public DateTime FechaHoy => DateTime.Today;

    #endregion

    #region 🔧 Implementación de IQueryAttributable
    /// <summary>
    /// Aplica los parámetros de navegación a la consulta
    /// </summary>
    /// <param name="query"></param>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("TarjetaId", out var tarjetaId) && tarjetaId is int id)
            TarjetaId = id;

        if (query.TryGetValue("NombreTarjeta", out var nombre))
            NombreTarjeta = (string)nombre;

        if (query.TryGetValue("TipoTarjeta", out var tipo))
            TipoTarjetaParam = (TipoTarjeta)tipo;

        // Actualiza la visibilidad del tab de ingresos para que se vea el boton de ingreso si es debito
        OnPropertyChanged(nameof(MostrarTabIngresos_Visible));
    }

    #endregion

    #region 🎯 Filtros

    /// Filtro activo: null = todos
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarTabTodos))]
    [NotifyPropertyChangedFor(nameof(MostrarTabGastos))]
    [NotifyPropertyChangedFor(nameof(MostrarTabIngresos))]
    private TipoTransaccion? _filtroActivo = null;

    public bool MostrarTabTodos => FiltroActivo is null;
    public bool MostrarTabGastos => FiltroActivo == TipoTransaccion.Gasto;
    public bool MostrarTabIngresos => FiltroActivo == TipoTransaccion.Ingreso;

    public bool MostrarTabIngresos_Visible => TipoTarjetaParam == TipoTarjeta.Debito;

    #endregion

    #region 🗑️ Eliminación (BottomSheet)

    [ObservableProperty]
    private TransaccionResumenDto? _transaccionAEliminar;

    [ObservableProperty]
    private bool _mostrarSheetEliminar = false;

    /// Abre el BottomSheet
    [RelayCommand]
    private void MostrarConfirmacionEliminar(TransaccionResumenDto transaccion)
    {
        TransaccionAEliminar = transaccion;
        MostrarSheetEliminar = true;
    }

    /// Confirma eliminación
    [RelayCommand]
    private async Task ConfirmarEliminarAsync()
    {
        MostrarSheetEliminar = false;

        if (TransaccionAEliminar is null) return;

        await EjecutarConCargaAsync(async () =>
        {
            await mediador.EnviarAsync(new EliminarTransaccionComando(TransaccionAEliminar.Id));
            TransaccionAEliminar = null;
            await CargarTransaccionesAsync();
        });
    }

    /// Cancela eliminación
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
        Titulo = NombreTarjeta;
        TipoTarjetaParam = TipoTarjetaParam == TipoTarjeta.Debito ? TipoTarjeta.Debito : TipoTarjeta.Credito;

        await CargarTransaccionesAsync();
        await CargarResumenCorteAsync();
    }

    #endregion

    #region 📦 Carga de datos

    /// Carga transacciones + calcula totales
    [RelayCommand]
    private async Task CargarTransaccionesAsync()
    {
        // Parametros:
        // - Func<Task>
        // - Mensaje Error --> Opcional
        await EjecutarConCargaAsync(async () =>
        {
            //Paso 1: Obtener transacciones segun la tarjeta y el filtro activo
            var resultado = await CargarTransaccionesSinCargaAsync();

            //Paso 3: Actualizar UI en lista de transacciones
            // Se puede hacer tambien new ObservableCollection<TransaccionResumenDto>(resultado)
            Transacciones = new (resultado);

            // Paso 4: Si no hay transacciones, mostrar estado vacío
            SinTransacciones = !resultado.Any();

            //Paso 5: Calcular totales de gastos e ingresos
            CalcularTotales(resultado);
        });
    }

    /// <summary>
    /// Cargar las transacciones por idTarjeta y filtroActivo
    /// </summary>
    /// <<returns></returns>>
    private async Task<List<TransaccionResumenDto>> CargarTransaccionesSinCargaAsync()
    {
        //Paso 1: Obtener transacciones segun la tarjeta y el filtro activo
        //Retorn --> IEnumerable<TransaccionResumenDto>
        var consulta = new ObtenerTransaccionesPorTarjetaConsulta(TarjetaId, FiltroActivo);

        //Paso 2: Ejecutar consulta
        return (await mediador.ConsultarAsync(consulta)).ToList();
    }

    /// <summary>
    /// Calcula el total de gastos e ingresos a partir 
    /// de la lista de transacciones obtenida, para mostrar en la UI
    /// </summary>
    /// <param name="resultado"></param>
    private void CalcularTotales(List<TransaccionResumenDto> resultado)
    {
        // Obtiene el total de gastos e ingresos para mostrar en la UI
        TotalGastos = resultado
            .Where(t => t.Tipo == TipoTransaccion.Gasto)
            .Sum(t => t.Monto);

        TotalIngresos = resultado
            .Where(t => t.Tipo == TipoTransaccion.Ingreso)
            .Sum(t => t.Monto);

        //Calcula el balance total
        Balance = TotalIngresos - TotalGastos;
    }

    private async Task CargarResumenCorteAsync()
    {
        //Esto solo carga si la tarjeta tiene DiaCorte configurado
        var resumen = await mediador.ConsultarAsync(
            new ObtenerResumenCorteConsulta(TarjetaId));

        // null si no aplica → IsVisible=False automáticamente
        ResumenCorte = resumen; 
    }

    #endregion

    #region 🎛️ Filtros

    /// Cambia el filtro (tabs)
    [RelayCommand]
    private async Task CambiarFiltroAsync(string filtro)
    {
        FiltroActivo = filtro switch
        {
            "gastos" => TipoTransaccion.Gasto,
            "ingresos" => TipoTransaccion.Ingreso,
            _ => null
        };

        await CargarTransaccionesAsync();
    }

    #endregion

    #region 🧭 Navegación

    /// Agregar gasto
    [RelayCommand]
    private async Task AgregarGastoAsync() =>
        await Shell.Current.GoToAsync(
            "transacciones/nueva",
            new Dictionary<string, object>
            {
                ["TarjetaId"] = TarjetaId,
                ["Tipo"] = TipoTransaccion.Gasto
            });

    /// Agregar ingreso
    [RelayCommand]
    private async Task AgregarIngresoAsync() =>
        await Shell.Current.GoToAsync(
            "transacciones/nueva",
            new Dictionary<string, object>
            {
                ["TarjetaId"] = TarjetaId,
                ["Tipo"] = TipoTransaccion.Ingreso
            });

    /// Editar transacción
    [RelayCommand]
    private async Task EditarTransaccionAsync(TransaccionResumenDto transaccion) =>
        await Shell.Current.GoToAsync(
            "transacciones/editar",
            new Dictionary<string, object>
            {
                ["TransaccionId"] = transaccion.Id,
                ["TarjetaId"] = TarjetaId,
                ["Tipo"] = transaccion.Tipo
            });

    #endregion
}