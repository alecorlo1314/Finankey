
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
            var guardarContrase�a = Preferences.Get("Contrasena", string.Empty);

            if (!string.IsNullOrEmpty(guardarUsuario) && !string.IsNullOrEmpty(guardarContrase�a))
            {
                // Si hay credenciales guardadas, iniciamos sesi�n autom�ticamente
                await Shell.Current.GoToAsync("detalles");
            }
        }
    }

    private async void OnLoginButtonClicked(object sender, EventArgs e)
    {
        //validar cedenciales (aqui deber�as implementar tu l�gica de autenticaci�n)
        bool isValid = ValidateCredentials(Usuario.Text, Contrasena.Text);

        if (isValid)
        {
            //guardar credenciales si el switch est� activado
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

            // Navegar a la p�gina principal
            Application.Current.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Error", "Credenciales inv�lidas. Por favor, int�ntalo de nuevo.", "OK");
        }
    }

    private bool ValidateCredentials(string username, string password)
    {
        // Implementa tu l�gica real de validaci�n aqu�
        return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
    }
}