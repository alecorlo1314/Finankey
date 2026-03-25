using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Aplicacion.Tarjetas.Comandos;
using FinanzasApp.Aplicacion.Tarjetas.Consultas;
using FinanzasApp.Domain.Enumeraciones;
using System.Collections.ObjectModel;

namespace FinanzasApp.Presentacion.ViewModels.Tarjetas;

[QueryProperty(nameof(TarjetaId), "TarjetaId")]
public partial class TarjetaFormViewModel(IMediator mediador) : ViewModelBase
{
    #region 🔧 Parámetros de navegación

    [ObservableProperty] private int _tarjetaId;

    #endregion

    #region 📝 Campos del formulario

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsFormularioValido))]
    private string _nombre = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsFormularioValido))]
    private string _ultimosDigitos = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarLimiteCredito))]
    private TipoTarjeta _tipoSeleccionado = TipoTarjeta.Debito;

    [ObservableProperty] private string _colorHex = "#3A86FF";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsFormularioValido))]
    private string _banco = string.Empty;

    [ObservableProperty] private string _redTarjeta = "Visa";
    [ObservableProperty] private string _limiteCredito = string.Empty;
    [ObservableProperty] private string _saldoActual = "0";

    #endregion

    #region 📊 Propiedades calculadas

    // Validación del formulario
    public bool EsFormularioValido =>
        !string.IsNullOrWhiteSpace(Nombre)
        && UltimosDigitos.Length == 4
        && UltimosDigitos.All(char.IsDigit)
        && !string.IsNullOrWhiteSpace(Banco);

    public bool EsModoEdicion => TarjetaId > 0;

    // Muestra campo de límite solo si es crédito
    public bool MostrarLimiteCredito => TipoSeleccionado == TipoTarjeta.Credito;

    #endregion

    #region 📚 Datos auxiliares (UI)

    // Redes disponibles
    public List<string> RedesTarjeta { get; } =
        ["Visa", "Mastercard", "American Express", "Discover"];

    // Colores disponibles para la tarjeta
    public ObservableCollection<string> ColoresDisponibles { get; } =
    [
        "#FF006E",
        "#3A86FF",
        "#8338EC",
    ];

    #endregion

    #region 🔄 Ciclo de vida

    public override async Task AlAparecerAsync()
    {
        Titulo = EsModoEdicion ? "Editar tarjeta" : "Nueva tarjeta";

        if (EsModoEdicion)
            await CargarDatosTarjetaAsync();
    }

    #endregion

    #region 🎯 Comandos

    /// Cambia el tipo de tarjeta (Crédito / Débito)
    [RelayCommand]
    private void SeleccionarTipo(string tipo)
    {
        TipoSeleccionado = tipo == "Credito"
            ? TipoTarjeta.Credito
            : TipoTarjeta.Debito;

        OnPropertyChanged(nameof(EsFormularioValido));
    }

    /// Guarda la tarjeta
    [RelayCommand(CanExecute = nameof(EsFormularioValido))]
    private async Task GuardarAsync()
    {
        await EjecutarConCargaAsync(async () =>
        {
            var dto = ConstruirDto();

            if (EsModoEdicion)
                await mediador.EnviarAsync(new ActualizarTarjetaComando(dto));
            else
                await mediador.EnviarAsync(new CrearTarjetaComando(dto));

            await Shell.Current.GoToAsync("..");
        });
    }

    #endregion

    #region 📦 Carga de datos

    /// Carga datos cuando se edita una tarjeta
    private async Task CargarDatosTarjetaAsync()
    {
        await EjecutarConCargaAsync(async () =>
        {
            var t = await mediador.ConsultarAsync(new ObtenerTarjetaPorIdConsulta(TarjetaId));
            if (t is null) return;

            Nombre = t.Nombre;
            UltimosDigitos = t.UltimosDigitos;
            TipoSeleccionado = t.Tipo;
            ColorHex = t.ColorHex;
            Banco = t.Banco;
            RedTarjeta = t.RedTarjeta;
            LimiteCredito = t.LimiteCredito?.ToString("F2") ?? string.Empty;
            SaldoActual = t.SaldoActual.ToString("F2");
        });
    }

    #endregion

    #region 🧩 Helpers

    /// Construye el DTO para enviar al backend
    private TarjetaFormDto ConstruirDto()
    {
        decimal.TryParse(LimiteCredito, out var limite);
        decimal.TryParse(SaldoActual, out var saldo);

        return new TarjetaFormDto(
            Id: EsModoEdicion ? TarjetaId : null,
            Nombre: Nombre.Trim(),
            UltimosDigitos: UltimosDigitos.Trim(),
            Tipo: TipoSeleccionado,
            ColorHex: ColorHex,
            Banco: Banco.Trim(),
            RedTarjeta: RedTarjeta,
            LimiteCredito: TipoSeleccionado == TipoTarjeta.Credito && limite > 0 ? limite : null,
            SaldoActual: saldo
        );
    }

    #endregion

    #region 🔁 Eventos

    // Reacciona cuando cambia el tipo (actualiza UI)
    partial void OnTipoSeleccionadoChanged(TipoTarjeta value)
    {
        OnPropertyChanged(nameof(MostrarLimiteCredito));
        OnPropertyChanged(nameof(EsFormularioValido));
    }

    #endregion
}