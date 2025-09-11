using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanKey.Dominio.Models;
using FinanKey.Aplicacion.UseCases;
using System.Collections.ObjectModel;

namespace FinanKey.Presentacion.ViewModels
{
    public partial class ViewModelGestionCategorias : ObservableObject
    {
        #region INYECCION DE DEPENDENCIAS

        private readonly ServicioCategoriaMovimiento _servicoCategoriaMovimiento;

        #endregion INYECCION DE DEPENDENCIAS

        #region PROPIEDADES OBSERVABLES

        // LISTAS - Cambiar a private set para mejor encapsulación
        [ObservableProperty]
        private ObservableCollection<CategoriaMovimiento> _listaCategoriaIngresos = new();

        [ObservableProperty]
        private ObservableCollection<CategoriaMovimiento> _listaCategoriaGastos = new();

        public ObservableCollection<Icono> ListaIconos { get; private set; } = new();

        // ESTADO UI
        [ObservableProperty]
        private bool _isBottomSheetOpen;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isGuardando;

        [ObservableProperty]
        private bool _popupEliminacionCategoriaAbierto;

        [ObservableProperty]
        public string _mensajeInformacion = string.Empty;

        [ObservableProperty]
        private bool _popupInformacionAbierto;

        [ObservableProperty]
        private bool actualizacionCategoria;

        //PROPIEDADES DE VISTA
        [ObservableProperty]
        public string? _nombreCategoria;

        [ObservableProperty]
        public CategoriaMovimiento? _CategoriaMovimientoSeleccionada;

        [ObservableProperty]
        public int _IdCategoriaSeleccionada;
        // CAMPOS FORMULARIO
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarCategoriaCommand))] // ✅ Auto-validación
        private string _descripcionCategoria = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GuardarCategoriaCommand))] // ✅ Auto-validación
        private Icono? _iconoSeleccionado;

        [ObservableProperty]
        private string _tipoMovimiento = "Ingreso"; // ✅ Valor por defecto corregido

        // ✅ PROPIEDADES CALCULADAS - Más eficiente que campos separados
        public bool NoHayCategoriasIngresos => !HayCategoriasIngresos;

        public bool HayCategoriasIngresos => ListaCategoriaIngresos?.Count > 0;
        public bool NoHayCategoriasGastos => !HayCategoriasGastos;
        public bool HayCategoriasGastos => ListaCategoriaGastos?.Count > 0;

        #endregion PROPIEDADES OBSERVABLES

        #region CONSTRUCTOR

        public ViewModelGestionCategorias(ServicioCategoriaMovimiento servicoCategoriaMovimiento)
        {
            _servicoCategoriaMovimiento = servicoCategoriaMovimiento ?? throw new ArgumentNullException(nameof(servicoCategoriaMovimiento));
            CargarIconos();

            //// ✅ Cargar datos iniciales
            _ = Task.Run(async () => await CargarDatosInicialesAsync());
        }

        #endregion CONSTRUCTOR

        #region CARGA DE DATOS INICIALES

        public async Task CargarDatosInicialesAsync()
        {
            try
            {
                IsLoading = true;
                await CargarTodasLasCategorias();
            }
            catch (Exception ex)
            {
                await MostrarError("Error cargando datos iniciales", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion CARGA DE DATOS INICIALES

        #region ICONOS - MAS MANTENIBLES

        private void CargarIconos()
        {
            var iconos = new[]
            {
                new Icono { Id = 1, Nombre = "Accesorio", Ruta = "icono_accesorio.svg" },
                new Icono { Id = 2, Nombre = "Avión", Ruta = "icono_avion.svg" },
                new Icono { Id = 3, Nombre = "Bebidas", Ruta = "icono_bebida.svg" },
                new Icono { Id = 4, Nombre = "Bus", Ruta = "icono_bus.svg" },
                new Icono { Id = 5, Nombre = "Café", Ruta = "icono_cafe.svg" },
                new Icono { Id = 6, Nombre = "Carro", Ruta = "icono_carro.svg" },
                new Icono { Id = 7, Nombre = "Casa", Ruta = "icono_casa.svg" },
                new Icono { Id = 8, Nombre = "Cine", Ruta = "icono_cine.svg" },
                new Icono { Id = 9, Nombre = "Corte", Ruta = "icono_corte.svg" },
                new Icono { Id = 10, Nombre = "Curso", Ruta = "icono_curso.svg" },
                new Icono { Id = 11, Nombre = "Deporte", Ruta = "icono_deporte.svg" },
                new Icono { Id = 12, Nombre = "Familia", Ruta = "icono_familia.svg" },
                new Icono { Id = 13, Nombre = "Gasolina", Ruta = "icono_gasolina.svg" },
                new Icono { Id = 14, Nombre = "Herramienta", Ruta = "icono_herramienta.svg" },
                new Icono { Id = 15, Nombre = "Higiene", Ruta = "icono_higiene.svg" },
                new Icono { Id = 16, Nombre = "Hospital", Ruta = "icono_hospital.svg" },
                new Icono { Id = 17, Nombre = "Internet", Ruta = "icono_internet.svg" },
                new Icono { Id = 18, Nombre = "Inversión", Ruta = "icono_inversion.svg" },
                new Icono { Id = 19, Nombre = "Libro", Ruta = "icono_libro.svg" },
                new Icono { Id = 20, Nombre = "Luz", Ruta = "icono_luz.svg" },
                new Icono { Id = 21, Nombre = "Mascota", Ruta = "icono_mascota.svg" },
                new Icono { Id = 22, Nombre = "Pastilla", Ruta = "icono_pastilla.svg" },
                new Icono { Id = 23, Nombre = "Regalo", Ruta = "icono_regalo.svg" },
                new Icono { Id = 24, Nombre = "Restaurante", Ruta = "icono_restaurante.svg" },
                new Icono { Id = 25, Nombre = "Salario", Ruta = "icono_salario.svg" },
                new Icono { Id = 26, Nombre = "Ropa", Ruta = "icono_ropa.svg" },
                new Icono { Id = 27, Nombre = "Serie", Ruta = "icono_serie.svg" },
                new Icono { Id = 28, Nombre = "Software", Ruta = "icono_software.svg" },
                new Icono { Id = 29, Nombre = "Supermercado", Ruta = "icono_supermercado.svg" },
                new Icono { Id = 30, Nombre = "Trabajo", Ruta = "icono_trabajo.svg" },
                new Icono { Id = 31, Nombre = "Videojuegos", Ruta = "icono_videojuegos.svg" },
            };

            ListaIconos.Clear();
            foreach (var icono in iconos)
            {
                ListaIconos.Add(icono);
            }
        }

        #endregion ICONOS - MAS MANTENIBLES

        #region COMMANDS

        /// <summary>
        /// Muestra el BottomSheet para agregar una nueva categoría
        /// </summary>
        [RelayCommand]
        private void MostrarBottomSheetAnadirCategoria()
        {
            LimpiarFormulario();
            IsBottomSheetOpen = true;
        }

        /// <summary>
        /// Cierra el BottomSheet
        /// </summary>
        [RelayCommand]
        private void CerrarBottomSheet()
        {
            IsBottomSheetOpen = false;
            LimpiarFormulario();
        }

        [RelayCommand]
        private async Task EditarCategoria(CategoriaMovimiento categoriaMovimiento)
        {
            IdCategoriaSeleccionada = categoriaMovimiento.Id;
            DescripcionCategoria = categoriaMovimiento.Descripcion.Trim();
            IconoSeleccionado = ListaIconos.FirstOrDefault(x => x.Id == categoriaMovimiento.Icon_id);
            TipoMovimiento = categoriaMovimiento.TipoMovimiento;
            ActualizacionCategoria = true;
            IsBottomSheetOpen = true;
        }

        /// <summary>
        /// Guarda la nueva categoría
        /// </summary>
        /// <returns></returns>
        [RelayCommand(CanExecute = nameof(PuedeGuardarCategoria))]
        private async Task GuardarCategoria()
        {
            if (IsGuardando) return;

            try
            {
                IsGuardando = true;

                var categoria = new CategoriaMovimiento
                {
                    Descripcion = DescripcionCategoria.Trim(),
                    Icon_id = IconoSeleccionado!.Id,
                    RutaIcono = IconoSeleccionado.Ruta,
                    TipoMovimiento = TipoMovimiento
                };
                //Preguntar se es actualizacion
                if (ActualizacionCategoria is true)
                {
                    var categoriaActualizar = new CategoriaMovimiento
                    {
                        Id = IdCategoriaSeleccionada,
                        Descripcion = DescripcionCategoria.Trim(),
                        Icon_id = IconoSeleccionado!.Id,
                        RutaIcono = IconoSeleccionado.Ruta,
                        TipoMovimiento = TipoMovimiento
                    };
                    var resultadoActualizacion = await _servicoCategoriaMovimiento.ActualizarAsync(categoriaActualizar);

                    if (resultadoActualizacion > 0)
                    {
                        MostrarExito($"'{DescripcionCategoria}' se actualizo exitosamente");

                        //Actualizar la lista
                        CargarTodasLasCategorias();
                        //ActualizarListaCategoria(categoriaActualizar);

                        IsBottomSheetOpen = false;
                        ActualizacionCategoria = false;
                        LimpiarFormulario();
                        return;
                    }
                    else
                    {
                        await MostrarError("Error", "No se pudo actualizar la categoría");
                        ActualizacionCategoria = false;
                        return;
                    }
                }

                var resultado = await _servicoCategoriaMovimiento.guardarCategoriaMovimiento(categoria);

                if (resultado > 0)
                {
                    MostrarExito($"'{DescripcionCategoria}' guardada exitosamente");

                    // ✅ Agregar a la lista correspondiente sin recargar todo
                    AgregarCategoriaALista(categoria);

                    IsBottomSheetOpen = false;
                    LimpiarFormulario();
                }
                else
                {
                    await MostrarError("Error", "No se pudo guardar la categoría");
                }
            }
            catch (Exception ex)
            {
                await MostrarError("Error guardando categoría", ex.Message);
            }
            finally
            {
                IsGuardando = false;
            }
        }

        /// <summary>
        /// Refresca las categorías
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task RefrescarCategorias()
        {
            try
            {
                IsLoading = true;
                await CargarTodasLasCategorias();
            }
            catch (Exception ex)
            {
                await MostrarError("Error refrescando", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Elimina una categoría de la lista
        /// </summary>
        /// <param name="categoria"></param>
        [RelayCommand]
        public void EliminarCategoria(CategoriaMovimiento categoria)
        {
            CategoriaMovimientoSeleccionada = categoria;
            NombreCategoria = categoria.Descripcion;
            PopupEliminacionCategoriaAbierto = true;
        }

        /// <summary>
        /// Cancela la eliminación de una categoría en el popup
        /// </summary>
        [RelayCommand]
        public void CancelarEliminarCategoria()
        {
            CategoriaMovimientoSeleccionada = null;
            PopupEliminacionCategoriaAbierto = false; // cerrar popup
        }

        /// <summary>
        /// Cierra el popup de información
        /// </summary>
        [RelayCommand]
        public void CerrarPopInformacion()
        {
            PopupInformacionAbierto = false; //CerrarPopInformacion
            _mensajeInformacion = string.Empty;
        }

        /// <summary>
        /// Confirma la eliminación de una categoría
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task ConfirmarEliminarCategoria()
        {
            if (CategoriaMovimientoSeleccionada == null) return;

            try
            {
                IsLoading = true;
                var resultado = await _servicoCategoriaMovimiento.EliminarCategoriaMovimiento(CategoriaMovimientoSeleccionada.Id);

                if (resultado > 0)
                {
                    RemoverCategoriaDeLista(CategoriaMovimientoSeleccionada);
                    PopupEliminacionCategoriaAbierto = false;
                    MostrarExito($"'{CategoriaMovimientoSeleccionada.Descripcion}' eliminada exitosamente");
                }
                else
                {
                    await MostrarError("Error", "No se pudo eliminar la categoría");
                }
            }
            catch (Exception ex)
            {
                await MostrarError("Error eliminando", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion COMMANDS

        #region MÉTODOS PRIVADOS

        private bool PuedeGuardarCategoria()
        {
            return !string.IsNullOrWhiteSpace(DescripcionCategoria) &&
                   IconoSeleccionado != null &&
                   !IsGuardando;
        }

        private async Task CargarTodasLasCategorias()
        {
            var todasCategorias = await _servicoCategoriaMovimiento.ObtenerTodasAsync();
            if (todasCategorias == null) return;

            //  Limpiar y llenar ambas listas eficientemente
            ListaCategoriaIngresos.Clear();
            ListaCategoriaGastos.Clear();

            foreach (var categoria in todasCategorias)
            {
                if (categoria.TipoMovimiento == "Ingreso")
                {
                    ListaCategoriaIngresos.Add(categoria);
                }
                else if (categoria.TipoMovimiento == "Gasto")
                {
                    ListaCategoriaGastos.Add(categoria);
                }
            }

            // ✅ Notificar cambios en propiedades calculadas
            OnPropertyChanged(nameof(HayCategoriasIngresos));
            OnPropertyChanged(nameof(NoHayCategoriasIngresos));
            OnPropertyChanged(nameof(HayCategoriasGastos));
            OnPropertyChanged(nameof(NoHayCategoriasGastos));
        }

        private void AgregarCategoriaALista(CategoriaMovimiento categoria)
        {
            // ✅ Asociar icono
            categoria.Icono = ListaIconos.FirstOrDefault(i => i.Id == categoria.Icon_id);

            if (categoria.TipoMovimiento == "Ingreso")
            {
                ListaCategoriaIngresos.Add(categoria);
                OnPropertyChanged(nameof(HayCategoriasIngresos));
                OnPropertyChanged(nameof(NoHayCategoriasIngresos));
            }
            else if (categoria.TipoMovimiento == "Gasto")
            {
                ListaCategoriaGastos.Add(categoria);
                OnPropertyChanged(nameof(HayCategoriasGastos));
                OnPropertyChanged(nameof(NoHayCategoriasGastos));
            }
        }

        private void RemoverCategoriaDeLista(CategoriaMovimiento categoria)
        {
            if (categoria.TipoMovimiento == "Ingreso")
            {
                ListaCategoriaIngresos.Remove(categoria);
                OnPropertyChanged(nameof(HayCategoriasIngresos));
                OnPropertyChanged(nameof(NoHayCategoriasIngresos));
            }
            else if (categoria.TipoMovimiento == "Gasto")
            {
                ListaCategoriaGastos.Remove(categoria);
                OnPropertyChanged(nameof(HayCategoriasGastos));
                OnPropertyChanged(nameof(NoHayCategoriasGastos));
            }
        }

        private void LimpiarFormulario()
        {
            DescripcionCategoria = string.Empty;
            IconoSeleccionado = null;
            TipoMovimiento = "Ingreso";
        }

        partial void OnTipoMovimientoChanged(string value)
        {
            // ✅ Reaccionar al cambio de tipo si es necesario
            System.Diagnostics.Debug.WriteLine($"Tipo movimiento cambiado a: {value}");
        }

        private async Task MostrarError(string titulo, string mensaje)
        {
            await Shell.Current.DisplayAlert(titulo, mensaje, "OK");
        }

        private void MostrarExito(string mensaje)
        {
            MensajeInformacion = mensaje;
            PopupInformacionAbierto = true;
        }

        #endregion MÉTODOS PRIVADOS
    }
}