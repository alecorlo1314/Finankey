using FinanKey.ViewModels;

namespace FinanKey.View;

public partial class AnadirPage : ContentPage
{
    //Inyección de dependencias para los ViewModels
    private readonly ViewModelGasto _viewModelGasto;
    private readonly ViewModelIngreso _viewModelIngreso;

    public AnadirPage(ViewModelGasto viewModelGasto, ViewModelIngreso viewModelIngreso)
    {
        InitializeComponent();
        // Asignar el BindingContext de la página a los ViewModels
        _viewModelGasto = viewModelGasto;
        _viewModelIngreso = viewModelIngreso;
        // Establecer el BindingContext inicial
        MostrarFormulario(FormularioSeleccionado.Gasto);
    }

    private enum FormularioSeleccionado
    {
        Gasto,
        Ingreso,
        Cuenta
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
                // Establecer el BindingContext para el formulario de Gasto
                BindingContext = _viewModelGasto;
                break;
            //Si selecciono formulario Ingreso
            case FormularioSeleccionado.Ingreso:
                // Mostrar el formulario de Ingreso
                formularioIngreso.IsVisible = true;
                // Estilizar el botón de Ingreso
                EstilizarBotonSeleccionado(btnIngreso, "icono_billete_blanco");
                // Establecer el BindingContext para el formulario de Ingreso
                BindingContext = _viewModelIngreso;
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

    private void btnCuenta_Pressed(object sender, EventArgs e)
    {
        MostrarFormulario(FormularioSeleccionado.Cuenta);
    }

    // Zona eventos de enfoque/desenfoque para entradas

    private void OnEntryFocused(Border border)
    {
        border.Stroke = App.Current?.Resources["ColorAzulPricipal"] as Color;
        border.StrokeThickness = 2;
    }

    private void OnEntryUnfocused(Border border)
    {
        border.Stroke = Colors.Transparent;
    }

    private void entradaTitulo_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderTitulo);
    private void entradaTitulo_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderTitulo);

    private void entradaMonto_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderMonto);
    private void entradaMonto_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderMonto);

    private void entradaTituloIngreso_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderTituloIngreso);
    private void entradaTituloIngreso_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderTituloIngreso);

    private void entradaDescripcionGasto_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderTitulo);
    private void entradaDescripcionGasto_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderTitulo);

    private void entradaMontoIngreso_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderMontoIngreso);
    private void entradaMontoIngreso_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderMontoIngreso);
}
