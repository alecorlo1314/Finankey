
namespace FinanKey.View;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

        //verificamos si hay credenciales guardadas
        RevisarCredencialesGuardadas();
    }

    private async void RevisarCredencialesGuardadas()
    {
        if (Preferences.ContainsKey("Recuerdame") && Preferences.Get("Recuerdame", false))
        {
            var guardarUsuario = Preferences.Get("Usuario", string.Empty);
            var guardarContraseña = Preferences.Get("Contrasena", string.Empty);

            if (!string.IsNullOrEmpty(guardarUsuario) && !string.IsNullOrEmpty(guardarContraseña))
            {
                // Si hay credenciales guardadas, iniciamos sesión automáticamente
                await Shell.Current.GoToAsync("detalles");
            }
        }
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        //validar cedenciales (aqui deberías implementar tu lógica de autenticación)
        bool isValid = ValidateCredentials(Usuario.Text, Contrasena.Text);

        if (isValid)
        {
            //guardar credenciales si el switch está activado
            if (Recuerdame.IsToggled)
            {
                Preferences.Set("Recuerdame", true);
                Preferences.Set("Usuario", Usuario.Text);
                Preferences.Set("Contrasena", Contrasena.Text);
            }
            else
            {
                Preferences.Remove("Recuerdame");
                Preferences.Remove("Usuario");
                Preferences.Remove("Contrasena");
            }

            // Navegar a la página principal
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Credenciales inválidas. Por favor, inténtalo de nuevo.", "OK");
        }
    }

    private bool ValidateCredentials(string username, string password)
    {
        // Implementa tu lógica real de validación aquí
        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }
}