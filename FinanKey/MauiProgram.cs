using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using Syncfusion.Maui.Core.Hosting;
using FinanKey.View;
using FinanKey.Servicios;
using FinanKey.ViewModels;
using FinanKey.Datos;
using Xe.AcrylicView;

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
                .UseAcrylicView()
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

            builder.Services.AddSingleton<AnadirPage>();
            builder.Services.AddSingleton<ConfiguracionesPage>();
            builder.Services.AddSingleton<FinanzasPage>();
            builder.Services.AddSingleton<DetalleCuentaPage>();
            builder.Services.AddTransient<LoginPage>();
            //Registrar ViewModels con inyección de dependencias
            builder.Services.AddSingleton<ViewModelGasto>();
            builder.Services.AddSingleton<ViewModelIngreso>();
            builder.Services.AddSingleton<ViewModelCuenta>();
            builder.Services.AddTransient<ViewModelLogin>();
            builder.Services.AddSingleton<ViewModelFinanzas>();
            builder.Services.AddSingleton<ViewModelDetalleCuenta>();
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

            return builder.Build();
        }
    }
}
