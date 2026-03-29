using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanzasApp.Aplicacion.DTOs;
using FinanzasApp.Aplicacion.Interfaces;
using FinanzasApp.Aplicacion.Transacciones.Comandos;
using FinanzasApp.Aplicacion.Transacciones.Consultas;
using FinanzasApp.Domain.Enumeraciones;
using FinanzasApp.Domain.Interfaces;
using FinanzasApp.Presentacion.Modelos;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Alerts;

namespace FinanzasApp.Presentacion.ViewModels.Transacciones;

[QueryProperty(nameof(TransaccionId), "TransaccionId")]
[QueryProperty(nameof(TarjetaIdParam), "TarjetaId")]
[QueryProperty(nameof(TipoParam), "Tipo")]
public partial class TransaccionFormViewModel(
    IMediator mediador,
    IServicioPrediccion servicioPrediccion) : ViewModelBase
{
    #region 🔧 Campos privados

    // Controla el debounce de la predicción IA
    private CancellationTokenSource? _ctsPrediccion;

    #endregion

    #region 📥 Parámetros de navegación (Shell)

    [ObservableProperty] private int _transaccionId;
    [ObservableProperty] private int _tarjetaIdParam;
    [ObservableProperty] private TipoTransaccion _tipoParam = TipoTransaccion.Gasto;

    #endregion

    #region 📝 Campos del formulario

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsFormularioValido))]
    private string _descripcion = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EsFormularioValido))]
    private string _monto = string.Empty;

    [ObservableProperty]
    private CategoriaTransaccion _categoriaSeleccionada = CategoriaTransaccion.Otros;

    [ObservableProperty]
    private DateTime _fechaSeleccionada = DateTime.Today;

    [ObservableProperty]
    private string _notas = string.Empty;

    #endregion

    #region 🤖 Estado de la IA

    [ObservableProperty] private bool _prediccionEnProceso;
    [ObservableProperty] private string _mensajePrediccion = string.Empty;
    [ObservableProperty] private float _confianzaPrediccion;
    [ObservableProperty] private bool _categoriaFuePredicha;
    [ObservableProperty] private bool _iaDisponible;

    #endregion

    #region 📊 Propiedades calculadas

    // Valida que el formulario esté listo para guardar
    public bool EsFormularioValido =>
        !string.IsNullOrWhiteSpace(Descripcion)
        && decimal.TryParse(Monto, out var m) && m > 0;

    public bool EsModoEdicion => TransaccionId > 0;

    #endregion

    #region 🏷️ Categorías (chips)

    [ObservableProperty]
    private ObservableCollection<CategoriaItem> _categoriasDisponibles = [];

    /// Inicializa las categorías según tipo (gasto/ingreso)
    private void InicializarCategorias()
    {
        var items = TipoParam == TipoTransaccion.Gasto
            ? new List<CategoriaItem>
            {
                new() { Categoria = CategoriaTransaccion.Alimentacion, Icono = "icono_alimentacion.png", Nombre = "Alimentación" },
                new() { Categoria = CategoriaTransaccion.Transporte, Icono = "icono_transporte.png", Nombre = "Transporte" },
                new() { Categoria = CategoriaTransaccion.Entretenimiento, Icono = "icono_entretenimiento.png", Nombre = "Entretenimiento" },
                new() { Categoria = CategoriaTransaccion.Salud, Icono = "icono_salud.svg", Nombre = "Salud" },
                new() { Categoria = CategoriaTransaccion.Educacion, Icono = "icono_educacion.svg", Nombre = "Educación" },
                new() { Categoria = CategoriaTransaccion.Hogar, Icono = "icono_hogar.svg", Nombre = "Hogar" },
                new() { Categoria = CategoriaTransaccion.Ropa, Icono = "icono_ropa.svg", Nombre = "Ropa" },
                new() { Categoria = CategoriaTransaccion.Tecnologia, Icono = "icono_tecnologia.svg", Nombre = "Tecnología" },
                new() { Categoria = CategoriaTransaccion.Viajes, Icono = "icono_viajes.svg", Nombre = "Viajes" },
                new() { Categoria = CategoriaTransaccion.Servicios, Icono = "icono_servicios.svg", Nombre = "Servicios" },
                new() { Categoria = CategoriaTransaccion.Restaurantes, Icono = "icono_restaurante.svg", Nombre = "Restaurantes" },
                new() { Categoria = CategoriaTransaccion.Deportes, Icono = "icono_deportes.svg", Nombre = "Deportes" },
                new() { Categoria = CategoriaTransaccion.Suscripciones, Icono = "icono_suscripcion.svg", Nombre = "Suscrip." },
                new() { Categoria = CategoriaTransaccion.Otros, Icono = "icono_otros.svg", Nombre = "Otros" },
            }
            : new List<CategoriaItem>
            {
                new() { Categoria = CategoriaTransaccion.SalarioSueldo, Icono = "icono_salario.svg", Nombre = "Salario" },
                new() { Categoria = CategoriaTransaccion.Freelance, Icono = "icono_freelance.svg", Nombre = "Freelance" },
                new() { Categoria = CategoriaTransaccion.Inversiones, Icono = "icono_inversiones.svg", Nombre = "Inversiones" },
                new() { Categoria = CategoriaTransaccion.Reembolsos, Icono = "icono_reembolso.svg", Nombre = "Reembolso" },
                new() { Categoria = CategoriaTransaccion.Otros, Icono = "icono_otros.svg", Nombre = "Otros" },
            };

        foreach (var item in items)
            item.EstaSeleccionada = item.Categoria == CategoriaSeleccionada;

        CategoriasDisponibles = new ObservableCollection<CategoriaItem>(items);
    }

    /// Selecciona una categoría manualmente
    [RelayCommand]
    private void SeleccionarCategoria(CategoriaItem item)
    {
        foreach (var c in CategoriasDisponibles)
            c.EstaSeleccionada = false;

        item.EstaSeleccionada = true;
        CategoriaSeleccionada = item.Categoria;

        // Ya no es predicción IA
        CategoriaFuePredicha = false;
    }

    // Sincroniza UI cuando cambia la categoría
    partial void OnCategoriaSeleccionadaChanged(CategoriaTransaccion value)
    {
        foreach (var c in CategoriasDisponibles)
            c.EstaSeleccionada = c.Categoria == value;
    }

    #endregion

    #region 🔄 Ciclo de vida

    public override async Task AlAparecerAsync()
    {
        Titulo = TipoParam == TipoTransaccion.Gasto
            ? (EsModoEdicion ? "Editar gasto" : "Nuevo gasto")
            : (EsModoEdicion ? "Editar ingreso" : "Nuevo ingreso");

        IaDisponible = servicioPrediccion.ModeloListo;

        InicializarCategorias();

        if (EsModoEdicion)
            await CargarTransaccionExistenteAsync();
    }

    #endregion

    #region 💾 Comandos principales

    [RelayCommand(CanExecute = nameof(EsFormularioValido))]
    private async Task GuardarAsync()
    {
        await EjecutarConCargaAsync(async () =>
        {
            //Paso 1: Mapear a TransaccionDTO
            var dto = ConstruirDto();

            //Paso 2: Verificar si es edición o creación
            if (EsModoEdicion)
            {
                //Paso 3: Enviar el comando para actualizar
                //Solo si estamos en modo edición
                var actualizado = await mediador.EnviarAsync(new ActualizarTransaccionComando(dto));
                if (!actualizado)
                {
                    await MostrarToastAsync("No se pudo actualizar la transaccion");
                    return;
                }
                await MostrarToastAsync("Transaccion actualizada");

                await Shell.Current.GoToAsync("..");
            }
            else
            {
                //Paso 3: Enviar el comando para crear
                //Solo si estamos en modo creación
                var creado = await mediador.EnviarAsync(new CrearTransaccionComando(dto));

                if (creado == 0)
                {
                    await MostrarToastAsync("No se pudo crear la transaccion");
                    return;
                }

                await MostrarToastAsync("Transaccion creada");

                //Limpiar campos
                LimpiarCampos();
            }
        });
    }

    [RelayCommand]
    private async Task PredecirCategoriaManualAsync() =>
        await EjecutarPrediccionAsync(Descripcion);

    #endregion

    #region 🤖 Lógica de predicción IA

    partial void OnDescripcionChanged(string value)
    {
        OnPropertyChanged(nameof(EsFormularioValido));

        if (!IaDisponible || string.IsNullOrWhiteSpace(value) || value.Length < 3)
        {
            MensajePrediccion = string.Empty;
            return;
        }

        _ctsPrediccion?.Cancel();
        _ctsPrediccion = new CancellationTokenSource();
        var token = _ctsPrediccion.Token;

        Task.Run(async () =>
        {
            await Task.Delay(600, token);
            if (!token.IsCancellationRequested)
                await MainThread.InvokeOnMainThreadAsync(() => EjecutarPrediccionAsync(value));
        }, token);
    }

    private async Task EjecutarPrediccionAsync(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion) || !IaDisponible) return;

        PrediccionEnProceso = true;
        MensajePrediccion = "Analizando...";

        try
        {
            var resultado = TipoParam == TipoTransaccion.Gasto
                ? await servicioPrediccion.PredecirCategoriaGastoAsync(descripcion)
                : await servicioPrediccion.PredecirCategoriaIngresoAsync(descripcion);

            ConfianzaPrediccion = resultado.Confianza;

            if (resultado.EsConfiable)
            {
                CategoriaSeleccionada = resultado.CategoriaPredicha;
                CategoriaFuePredicha = true;

                var porcentaje = (int)(resultado.Confianza * 100);
                MensajePrediccion = $"✨ IA sugiere: {NombreCategoria(resultado.CategoriaPredicha)} ({porcentaje}%)";
            }
            else
            {
                CategoriaFuePredicha = false;
                MensajePrediccion = "IA no pudo determinar la categoría";
            }
        }
        catch
        {
            MensajePrediccion = string.Empty;
        }
        finally
        {
            PrediccionEnProceso = false;
        }
    }

    #endregion

    #region 📦 Carga de datos

    private async Task CargarTransaccionExistenteAsync()
    {
        await EjecutarConCargaAsync(async () =>
        {
            var t = await mediador.ConsultarAsync(new ObtenerTransaccionPorIdConsulta(TransaccionId));
            if (t is null) return;

            Descripcion = t.Descripcion;
            Monto = t.Monto.ToString("F2");
            CategoriaSeleccionada = t.Categoria;
            FechaSeleccionada = t.Fecha;
            Notas = t.Notas ?? string.Empty;
            CategoriaFuePredicha = t.CategoriaPredicha;
            ConfianzaPrediccion = t.ConfianzaPrediccion;
        });
    }

    #endregion

    #region 🧩 Helpers

    private static string NombreCategoria(CategoriaTransaccion categoria) => categoria switch
    {
        CategoriaTransaccion.Alimentacion => "Alimentación",
        CategoriaTransaccion.Transporte => "Transporte",
        CategoriaTransaccion.Entretenimiento => "Entretenimiento",
        CategoriaTransaccion.Salud => "Salud",
        CategoriaTransaccion.Educacion => "Educación",
        CategoriaTransaccion.Hogar => "Hogar",
        CategoriaTransaccion.Ropa => "Ropa",
        CategoriaTransaccion.Tecnologia => "Tecnología",
        CategoriaTransaccion.Viajes => "Viajes",
        CategoriaTransaccion.Servicios => "Servicios",
        CategoriaTransaccion.Restaurantes => "Restaurantes",
        CategoriaTransaccion.Deportes => "Deportes",
        CategoriaTransaccion.Suscripciones => "Suscripciones",
        CategoriaTransaccion.SalarioSueldo => "Salario / Sueldo",
        CategoriaTransaccion.Freelance => "Freelance",
        CategoriaTransaccion.Inversiones => "Inversiones",
        CategoriaTransaccion.Reembolsos => "Reembolsos",
        _ => "Otros"
    };

    [RelayCommand]
    private void VerTodasCategorias()
    {
        // Aquí puedes abrir un BottomSheet o expandir lista
    }

    private TransaccionFormDto ConstruirDto()
    {
        decimal.TryParse(Monto, out var montoDecimal);

        return new TransaccionFormDto(
                Id: EsModoEdicion ? TransaccionId : null,
                TarjetaId: TarjetaIdParam,
                Descripcion: Descripcion.Trim(),
                Monto: montoDecimal,
                Tipo: TipoParam,
                Categoria: CategoriaSeleccionada,
                CategoriaPredicha: CategoriaFuePredicha,
                ConfianzaPrediccion: ConfianzaPrediccion,
                Fecha: FechaSeleccionada,
                Notas: string.IsNullOrWhiteSpace(Notas) ? null : Notas.Trim()
            );
    }

    private void LimpiarCampos()
    {
        Descripcion = string.Empty;
        Monto = string.Empty;
        CategoriaSeleccionada = CategoriaTransaccion.Otros;
        Notas = string.Empty;
    }

    #endregion
    }