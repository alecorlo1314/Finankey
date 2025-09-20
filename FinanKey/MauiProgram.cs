using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Syncfusion.Maui.Core.Hosting;
using SimpleToolkit.SimpleShell;
using SimpleToolkit.Core;
using FinanKey.Presentacion.ViewModels;
using FinanKey.Infraestructura.Repositorios;
using FinanKey.Presentacion.View;
using FinanKey.Dominio.Interfaces;
using FinanKey.Presentacion.View.Behaviors;
using FinanKey.Aplicacion.UseCases;

namespace FinanKey
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
#endif
            });
            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
#endif
            });

            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureSyncfusionCore()
                .UseSimpleShell()
                .UseSimpleToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    //Fuentes Poppins
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemibold");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-Medium.ttf", "PoppinsMedium");
                    fonts.AddFont("Poppins-Light.ttf", "PoppinsLight");
                });
            builder.UseSimpleToolkit();
            //Registrar vistas con inyección de dependencias
            builder.Services.AddSingleton<AgregarTarjetaPage>();
            builder.Services.AddSingleton<AnadirPage>();
            builder.Services.AddSingleton<ConfiguracionesPage>();
            builder.Services.AddSingleton<InicioPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<ReportesPage>();
            builder.Services.AddSingleton<GestionCategoriaPage>();
            builder.Services.AddSingleton<DetalleTarjetaPage>();
            //Registrar ViewModels con inyección de dependencias
            builder.Services.AddSingleton<ViewModelInicio>();
            builder.Services.AddSingleton<ViewModelMovimiento>();
            builder.Services.AddSingleton<ViewModelTarjeta>();
            builder.Services.AddSingleton<ViewModelAjustes>();
            builder.Services.AddSingleton<ViewModelGestionCategorias>();
            builder.Services.AddSingleton<ViewModelDetalleTarjeta>();
            //Registrar servicios con inyección de dependencias en Respositorios
            builder.Services.AddSingleton<RespositorioMovimiento>();
            builder.Services.AddSingleton<RepositorioTarjeta>();
            builder.Services.AddSingleton<RepositorioCategoriaMovimiento>();
            builder.Services.AddSingleton<RepositorioBaseDatos>();
            //Registrar Interfaz de servicios de Interfaz con inyección de dependencias
            builder.Services.AddSingleton<IServicioMovimiento, RespositorioMovimiento>();
            builder.Services.AddSingleton<IServicioTarjeta, RepositorioTarjeta>();
            builder.Services.AddSingleton<IServicioCategoriaMovimiento, RepositorioCategoriaMovimiento>();
            builder.Services.AddSingleton<IServicioBaseDatos, RepositorioBaseDatos>();
            //Registrar Interfaz de servicios de Interfaz con inyección de dependencias
            builder.Services.AddSingleton<ServicioMovimiento>();
            builder.Services.AddSingleton<ServicioTarjeta>();
            builder.Services.AddSingleton<ServicioInicio>();
            builder.Services.AddSingleton<ServicioCategoriaMovimiento>();
            builder.Services.AddSingleton<ServicioDetallesTarjeta>();
            //Registrar comportamiento personalizado Behavior
            builder.Services.AddSingleton<SfRadioButtonStateChangedBehavior>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if ANDROID

            builder.SetDefaultNavigationBarAppearance(Color.FromArgb("#FFFFFF"));

#endif

            builder.ConfigureMauiHandlers(handlers => { });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}