using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Syncfusion.Maui.Core.Hosting;
using FinanKey.View;
using FinanKey.Servicios;
using FinanKey.ViewModels;
using FinanKey.Datos;
using SimpleToolkit.SimpleShell;
using SimpleToolkit.Core;

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
                });
            builder.UseSimpleToolkit();
            builder.Services.AddSingleton<AnadirPage>();
            builder.Services.AddSingleton<ConfiguracionesPage>();
            builder.Services.AddSingleton<InicioPage>();
            builder.Services.AddSingleton<DetalleCuentaPage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddSingleton<AgregarTarjetaPage>();
            //Registrar ViewModels con inyección de dependencias
            builder.Services.AddSingleton<ViewModelGasto>();
            builder.Services.AddSingleton<ViewModelIngreso>();
            builder.Services.AddSingleton<ViewModelCuenta>();
            builder.Services.AddSingleton<ViewModelFinanzas>();
            builder.Services.AddSingleton<ViewModelDetalleCuenta>();
            builder.Services.AddSingleton<ViewModelTarjeta>();
            //Registrar servicios con inyección de dependencias contexto de datos
            builder.Services.AddSingleton<ContextoDatosCuenta>();
            builder.Services.AddSingleton<ContextoDatosGasto>();
            builder.Services.AddSingleton<ContextoDatosIngreso>();
            //Registrar servicios con contexto de datos
            builder.Services.AddSingleton<IServiciosTransaccionIngreso, ContextoDatosIngreso>();
            builder.Services.AddSingleton<IServiciosTransaccionGasto, ContextoDatosGasto>();
            builder.Services.AddSingleton<IServicioTransaccionCuenta, ContextoDatosCuenta>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if ANDROID || IOS

        builder.DisplayContentBehindBars();
#endif

#if ANDROID

 builder.SetDefaultNavigationBarAppearance(Color.FromArgb("#7A69EE"));

#endif
            return builder.Build();
        }
    }
}