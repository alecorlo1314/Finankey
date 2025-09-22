using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

public partial class AnadirPage : ContentPage
{
    private ViewModelMovimiento _viewModelMovimiento;
    public AnadirPage(ViewModelMovimiento viewModelMovimiento)
    {
        InitializeComponent();
        //inyeccion de dependencias
        _viewModelMovimiento = viewModelMovimiento;
        BindingContext = _viewModelMovimiento;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _ = _viewModelMovimiento.InicializarDatosAsync();
    }

    private enum FormularioSeleccionado
    {
        Gasto,
        Ingreso
    }

    private void MostrarFormulario(FormularioSeleccionado seleccionado)
    {
        // Ocultar todos los formularios
        formularioGasto.IsVisible = false;
        formularioIngreso.IsVisible = false;

        // Resetear estilos
        ResetearEstilosBotones();

        switch (seleccionado)
        {
            //Si selecciono formulario Gasto
            case FormularioSeleccionado.Gasto:
                // Mostrar el formulario de Gasto
                formularioGasto.IsVisible = true;
                // Estilizar el botón de Gasto
                EstilizarBotonSeleccionado(btnGasto, "icono_billetera_blanco.svg");
                break;
            //Si selecciono formulario Ingreso
            case FormularioSeleccionado.Ingreso:
                // Mostrar el formulario de Ingreso
                formularioIngreso.IsVisible = true;
                // Estilizar el botón de Ingreso
                EstilizarBotonSeleccionado(btnIngreso, "icono_billete_blanco");
                break;
        }
    }
    //Cuando el boton se presiona las interfaz del boton cambia de estilo
    private void EstilizarBotonSeleccionado(Button boton, string icono)
    {
        boton.BackgroundColor = App.Current?.Resources["ColorAzulPricipal"] as Color;
        boton.TextColor = Colors.White;
        boton.ImageSource = ImageSource.FromFile(icono);
    }

    private void ResetearEstilosBotones()
    {
        EstilizarBotonDesactivado(btnGasto, "icono_billetera_azul.svg");
        EstilizarBotonDesactivado(btnIngreso, "icono_billete_azul.svg");
    }
    //Cuando el boton no esta seleccionado se cambia el estilo
    private void EstilizarBotonDesactivado(Button boton, string icono)
    {
        boton.BackgroundColor = App.Current?.Resources["ColorFondoComponentes"] as Color;
        boton.TextColor = App.Current?.Resources["ColorAzulPricipal"] as Color;
        boton.ImageSource = ImageSource.FromFile(icono);
    }

    //Zona de eventos de los botones

    private void btnGasto_Pressed(object sender, EventArgs e)
    {
        MostrarFormulario(FormularioSeleccionado.Gasto);
    }

    private void btnIngreso_Pressed(object sender, EventArgs e)
    {
        MostrarFormulario(FormularioSeleccionado.Ingreso);
    }

    private void OnEntryFocused(Border border)
    {
        border.Stroke = App.Current?.Resources["ColorAzulPricipal"] as Color;
        border.StrokeThickness = 2;
    }

    private void OnEntryUnfocused(Border border)
    {
        border.Stroke = Colors.Transparent;
    }
    private void entradaMontoGasto_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderMontoGasto);
    private void entradaMontoGasto_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderMontoGasto);
    private void entradaMontoIngreso_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderMontoIngreso);
    private void entradaMontoIngreso_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderMontoIngreso);
    private void entradaDescripcionIngreso_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderMontoIngreso);
    private void entradaDescripcionIngreso_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderMontoIngreso);
    private void entradaDescripcionGasto_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderDescripcionGasto);
    private void entradaDescripcionGasto_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderDescripcionGasto);
    private void entradaComercio_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderComercio);
    private void entradaComercio_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderComercio);
    private void Button_Clicked(object sender, EventArgs e)
    {
        bottomSheetCategoria.Show();
    }

    private void btnGasto_Clicked(object sender, EventArgs e) => MostrarFormulario(FormularioSeleccionado.Gasto);

    private void btnIngreso_Clicked(object sender, EventArgs e) => MostrarFormulario(FormularioSeleccionado.Ingreso);
}
