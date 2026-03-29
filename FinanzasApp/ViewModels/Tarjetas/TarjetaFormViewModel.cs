using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Aplicacion.Tarjetas.Comandos;
using FinanzasApp.Aplicacion.Tarjetas.Consultas;
using FinanzasApp.Domain.Enumeraciones;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace FinanzasApp.Presentacion.ViewModels.Tarjetas;

[QueryProperty(nameof(TarjetaId), "TarjetaId")]
public partial class TarjetaFormViewModel(IMediator mediador) : ViewModelBase
{
    #region 🔧 Parámetros de navegación

    [ObservableProperty] private int _tarjetaId;

    #endregion

    #region 📝 Datos principales

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsFormularioValido))]
    private string? _nombre = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsFormularioValido))]
    private string? _banco = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsFormularioValido))]
    private string? _ultimosDigitos = string.Empty;

    [ObservableProperty]
    private string? _redTarjeta = "Visa";

    #endregion

    #region 💳 Configuración de tarjeta

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MostrarLimiteCredito))]
    private TipoTarjeta _tipoSeleccionado = TipoTarjeta.Debito;

    [ObservableProperty]
    private string? _colorHex = "#3A86FF";

    #endregion

    #region 💰 Datos financieros

    [ObservableProperty]
    private string? _saldoActual = "0";

    [ObservableProperty]
    private string? _limiteCredito = string.Empty;

    #endregion

    #region 📅 Fechas

    [ObservableProperty]
    private string? _mesVencimiento = string.Empty;

    [ObservableProperty]
    private string? _anioVencimiento = string.Empty;

    [ObservableProperty]
    private string? _diaCorte = string.Empty;

    [ObservableProperty]
    private string? _diaPago = string.Empty;

    #endregion

    #region ⚠️ Errores de validación

    [ObservableProperty] private string? _descripcionError;
    [ObservableProperty] private string? _bancoError;
    [ObservableProperty] private string? _ultimosDigitosError;
    [ObservableProperty] private string? _mesVencimientoError;
    [ObservableProperty] private string? _anioVencimientoError;
    [ObservableProperty] private string? _saldoActualError;
    [ObservableProperty] private string? _limiteCreditoError;

    #endregion

    #region 📊 Propiedades calculadas

    // Validación del formulario
    public bool EsFormularioValido
    {
        get
        {
            var cultura = CultureInfo.CurrentCulture;

            // Nombre
            if (string.IsNullOrWhiteSpace(Nombre) || Nombre.Trim().Length < 3)
                return false;

            // Banco
            if (string.IsNullOrWhiteSpace(Banco) || Banco.Trim().Length < 2)
                return false;

            // Últimos dígitos
            if (string.IsNullOrWhiteSpace(UltimosDigitos) || UltimosDigitos.Length != 4 || !UltimosDigitos.All(char.IsDigit))
                return false;

            // Mes de vencimiento (1-12)
            if (!int.TryParse(MesVencimiento, NumberStyles.Integer, cultura, out var mes) ||
                mes < 1 || mes > 12)
                return false;

            // Año de vencimiento (4 dígitos preferible)
            if (!int.TryParse(AnioVencimiento, NumberStyles.Integer, cultura, out var anio))
                return false;

            // Validar que la tarjeta no esté vencida (considerando fin de mes)
            try
            {
                // Si el año viene en 2 dígitos, podrías normalizarlo aquí si lo deseas.
                var ultimoDiaDelMes = DateTime.DaysInMonth(anio, mes);
                var fechaVencimiento = new DateTime(anio, mes, ultimoDiaDelMes, 23, 59, 59);
                if (fechaVencimiento < DateTime.Now)
                    return false;
            }
            catch
            {
                // Si la combinación mes/año no forma una fecha válida
                return false;
            }

            // Saldo actual (decimal) — usar la cultura que corresponda a tu app
            if (!decimal.TryParse(SaldoActual,
                                  NumberStyles.Number | NumberStyles.AllowCurrencySymbol,
                                  cultura,
                                  out _))
                return false;

            // Validar límite si es crédito
            if (TipoSeleccionado == TipoTarjeta.Credito)
            {
                if (!decimal.TryParse(LimiteCredito,
                    NumberStyles.Number,
                    cultura,
                    out var limite) || limite <= 0)
                    return false;
            }

            // Validar corte y pago si es crédito
            if (TipoSeleccionado == TipoTarjeta.Credito)
            {
                if (!int.TryParse(DiaCorte, out var corte) || corte < 1 || corte > 31)
                    return false;

                if (!int.TryParse(DiaPago, out var pago) || pago < 1 || pago > 31)
                    return false;
            }

            return true;
        }
    }

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
        if(!MostrarLimiteCredito) LimpiezaCampos();

        OnPropertyChanged(nameof(EsFormularioValido));
    }

    /// Guarda la tarjeta
    [RelayCommand(CanExecute = nameof(EsFormularioValido))]
    private async Task GuardarAsync()
    {
        await EjecutarConCargaAsync(async () =>
        {
            //Paso 1: Mapear los datos a un TarjetaFormDto
            var dto = ConstruirDto();

            //Paso 2: Verificar si es edición o creación
            if (EsModoEdicion)
            {
                //Paso 3: Enviar el comando para actualizar
                //Solo si estamos en modo edición
                var actualizado = await mediador.EnviarAsync(new ActualizarTarjetaComando(dto));

                if (!actualizado)
                {
                    await MostrarToastAsync("No se pudo actualizar la tarjeta");
                    return;
                }

                await MostrarToastAsync("Tarjeta actualizada");
            }
            else
            {
                //Paso 3: Enviar el comando para crear
                //Solo si estamos en modo creación
                var creado = await mediador.EnviarAsync(new CrearTarjetaComando(dto));

                if (creado == 0)
                {
                    await MostrarToastAsync("No se pudo crear la tarjeta");
                    return;
                }

                await MostrarToastAsync("Tarjeta creada");
            }

            //Paso 4: Navegar a la pantalla anterior
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

            var cultura = CultureInfo.CurrentCulture;

            TipoSeleccionado = t.Tipo;

            Nombre = t.Nombre;
            Banco = t.Banco;
            UltimosDigitos = t.UltimosDigitos;
            MesVencimiento = t.MesVencimiento.ToString();
            AnioVencimiento = t.AnioVencimiento.ToString();
            SaldoActual = t.SaldoActual.ToString("F2", cultura);

            LimiteCredito = t.Tipo == TipoTarjeta.Credito
                ? t.LimiteCredito?.ToString("F2", cultura) ?? string.Empty
                : string.Empty;

            DiaCorte = t.Tipo == TipoTarjeta.Credito
                ? t.DiaCorte?.ToString() ?? string.Empty
                : string.Empty;

            DiaPago = t.Tipo == TipoTarjeta.Credito
                ? t.DiaPago?.ToString() ?? string.Empty
                : string.Empty;

            ColorHex = t.ColorHex;
            RedTarjeta = t.RedTarjeta;
        });
    }

    #endregion

    #region 🧩 Helpers

    /// Construye el DTO para enviar al backend
    private TarjetaFormDto ConstruirDto()
    {
        //Parsear valores 
        decimal.TryParse(LimiteCredito, out var limite);
        decimal.TryParse(SaldoActual, out var saldo);
        int.TryParse(MesVencimiento, out var mesVencimiento);
        int.TryParse(AnioVencimiento, out var anioVencimiento);
        int.TryParse(DiaCorte, out var diaCorte);
        int.TryParse(DiaPago, out var diaPago);

        return new TarjetaFormDto(
            Id: EsModoEdicion ? TarjetaId : null,
            Nombre: Nombre.Trim(),
            UltimosDigitos: UltimosDigitos.Trim(),
            Tipo: TipoSeleccionado,
            ColorHex: ColorHex,
            Banco: Banco.Trim(),
            RedTarjeta: RedTarjeta,
            LimiteCredito: TipoSeleccionado == TipoTarjeta.Credito && limite > 0 ? limite : null,
            SaldoActual: saldo,
            MesVencimiento: mesVencimiento,
            AnioVencimiento: anioVencimiento,
            DiaCorte: diaCorte,
            DiaPago: diaPago
        );
    }

    //Limpieza de campos al cambiar Credito / Debito
    private void LimpiezaCampos()
    {
        LimiteCredito = string.Empty;
        DiaCorte = string.Empty;
        DiaPago = string.Empty;
    }

    #endregion

    #region 🔁 Eventos

    // Reacciona cuando cambia el tipo (actualiza UI)
    partial void OnTipoSeleccionadoChanged(TipoTarjeta value)
    {
        OnPropertyChanged(nameof(MostrarLimiteCredito));
        OnPropertyChanged(nameof(EsFormularioValido));
    }

    //Notifica cambios en las entradas cada ves que se actualiza una 
    private void ActualizarEstadoFormulario()
    {
        OnPropertyChanged(nameof(EsFormularioValido));
        GuardarCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region 📌 Validacion UI
    partial void OnNombreChanged(string value)
    {
        //Validacimos el nombre de la tarjeta
        ValidacionNombreTarjeta(value);
        ActualizarEstadoFormulario();
    }
    public void ValidacionNombreTarjeta(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            DescripcionError = "No puede estar vacía.";
            return;
        }
        if (value.Length < 3)
        {
            DescripcionError = "No puede tener menos de 3 caracteres.";
            return;
        }
        // Si pasa validación, limpiar errores
        DescripcionError = string.Empty;
    }

    partial void OnBancoChanged(string value)
    {
        //Validacimos el nombre de la tarjeta
        ValidacionNombreBanco(value);
        ActualizarEstadoFormulario();
    }
    public void ValidacionNombreBanco(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            BancoError = "No puede estar vacía.";
            return;
        }
        if (value.Length <= 2)
        {
            BancoError = "No puede tener menos de 2 caracteres.";
            return;
        }
        // Si pasa validación, limpiar errores
        BancoError = string.Empty;
    }

    partial void OnUltimosDigitosChanged(string value)
    {
        ValidarUltimosCuatroDigitos(value);
        ActualizarEstadoFormulario();
    }
    private void ValidarUltimosCuatroDigitos(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            UltimosDigitosError = "No puede estar vacía.";
            return;
        }
        if(value.Length < 4)
        {
            UltimosDigitosError = "No puede tener menos de 4 caracteres.";
            return;
        }
        if(!value.All(char.IsDigit))
        {
            UltimosDigitosError = "Solo se aceptan numeros.";
            return;
        }

        // Si pasa validación, limpiar errores
        UltimosDigitosError = string.Empty;
    }

    partial void OnMesVencimientoChanged(string value)
    {
        ValidarMesVencimiento(value);
        ActualizarEstadoFormulario();
    }
    private void ValidarMesVencimiento(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            MesVencimientoError = "No puede estar vacía.";
            return;
        }
        if (!value.All(char.IsDigit))
        {
            MesVencimientoError = "Solo se aceptan numeros.";
            return;
        }
        if (int.Parse(value) < 1 || int.Parse(value) > 12)
        {
            MesVencimientoError = "Solo 1 - 12.";
            return;
        }

        // Si pasa validación, limpiar errores
        MesVencimientoError = string.Empty;
    }

    partial void OnAnioVencimientoChanged(string value)
    {
        ValidarAnioVencimiento(value);
        ActualizarEstadoFormulario();
    }
    private void ValidarAnioVencimiento(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AnioVencimientoError = "No puede estar vacía.";
            OnPropertyChanged(nameof(EsFormularioValido));
            return;
        }
        if (!value.All(char.IsDigit))
        {
            AnioVencimientoError = "Solo se aceptan numeros.";
            return;
        }

        var anioActual = int.Parse(DateTime.Now.ToString("yyyy"));

        if (int.Parse(value) < anioActual)
        {
            AnioVencimientoError = "No puede ser menor al año actual.";
            return;
        }
        // Si pasa validación, limpiar errores
        AnioVencimientoError = string.Empty;
    }

    partial void OnSaldoActualChanged(string value)
    {
        ValidarSaldoActual(value);
        ActualizarEstadoFormulario();
    }
    private void ValidarSaldoActual(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            SaldoActualError = "No puede estar vacía.";
            return;
        }
        if (!decimal.TryParse(value, out _))
        {
            SaldoActualError = "Solo se aceptan numeros.";
            return;
        }

        // Si pasa validación, limpiar errores
        SaldoActualError = string.Empty;
    }

    //Validaciones solo para (Credito)
    partial void OnLimiteCreditoChanged(string value)
    {
        ValidarLimiteCreditoActual(value);
        ActualizarEstadoFormulario();
    }
    private void ValidarLimiteCreditoActual(string value)
    {
        //Paso 1: Verificar Credito este seleccionado
        //Si no es Credito, no pasa validación
        if (!MostrarLimiteCredito) return;

        //Paso 2: Verificar que no este vacio
        if (string.IsNullOrWhiteSpace(value))
        {
            LimiteCreditoError = "No puede estar vacía.";
            return;
        }

        //Paso 3: Verificar que sea un valor decimal
        if(!decimal.TryParse(value, out _))
        {
            LimiteCreditoError = "Solo se aceptan numeros.";
            return;
        }

        //Paso 4: No puede ser negativo
        if (decimal.Parse(value) < 0)
        {
            LimiteCreditoError = "El límite no puede ser negativo.";
            return;
        }
        //Paso 4: No puede ser igual a 0
        if (decimal.Parse(value) == 0)
        {
            LimiteCreditoError = "El límite debe ser mayor que cero para tarjetas de crédito.";
            return;
        }

        LimiteCreditoError = string.Empty;
    }
    #endregion
}