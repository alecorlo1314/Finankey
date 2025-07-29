using FinanKey.View;
using System.Threading.Tasks;

namespace FinanKey
{
    public partial class App : Application
    {
        public App()
        {
            //Licencia Syncfusion
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHZXQmJdUkd2V0JWYEk=");
            InitializeComponent();

            //verificamos si hay credenciales guardadas al iniciar la aplicación
            if(Preferences.ContainsKey("Recuerdame") && Preferences.Get("Recuerdame", false))
            {
                // Si hay credenciales guardadas, iniciamos sesión automáticamente
                MainPage = new AppShell();
            }
            else
            {
                // Si no hay credenciales guardadas, mostramos la página de inicio de sesión
                MainPage = new LoginPage();
            }
        }
        //Metodo para cerrar la sesion
        public static async Task Logout()
        {
            // Limpiar las preferencias guardadas
            Preferences.Remove("Recuerdame");
            Preferences.Remove("Usuario");
            Preferences.Remove("Contrasena");
            // Navegar de nuevo a la página de inicio de sesión
            await Shell.Current.GoToAsync("loginPage");
        }
    }
}
