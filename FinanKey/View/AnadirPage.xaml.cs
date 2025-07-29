using FinanKey.ViewModels;

namespace FinanKey.View;

public partial class AnadirPage : ContentPage
{
    //Inyección de dependencias para los ViewModels
    private readonly ViewModelGasto _viewModelGasto;
    private readonly ViewModelIngreso _viewModelIngreso;
    private readonly ViewModelCuenta _viewModelCuenta;

    public AnadirPage(ViewModelGasto viewModelGasto, ViewModelIngreso viewModelIngreso, ViewModelCuenta viewModelCuenta)
    {
        InitializeComponent();
        // Asignar el BindingContext de la página a los ViewModels
        _viewModelGasto = viewModelGasto;
        _viewModelIngreso = viewModelIngreso;
        _viewModelCuenta = viewModelCuenta;
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
        formularioCuenta.IsVisible = false;

        // Resetear estilos
        ResetearEstilosBotones();

        switch (seleccionado)
        {
            //Si selecciono formulario Gasto
            case FormularioSeleccionado.Gasto:
                // Mostrar el formulario de Gasto
                formularioGasto.IsVisible = true;
                // Estilizar el botón de Gasto
                EstilizarBotonSeleccionado(btnGasto, "cartera_blanca.svg");
                // Establecer el BindingContext para el formulario de Gasto
                BindingContext = _viewModelGasto;
                break;
            //Si selecciono formulario Ingreso
            case FormularioSeleccionado.Ingreso:
                // Mostrar el formulario de Ingreso
                formularioIngreso.IsVisible = true;
                // Estilizar el botón de Ingreso
                EstilizarBotonSeleccionado(btnIngreso, "billete_blanco.svg");
                // Establecer el BindingContext para el formulario de Ingreso
                BindingContext = _viewModelIngreso;
                break;
            //Si selecciono formulario Cuenta
            case FormularioSeleccionado.Cuenta:
                // Mostrar el formulario de Cuenta
                formularioCuenta.IsVisible = true;
                // Estilizar el botón de Cuenta
                EstilizarBotonSeleccionado(btnCuenta, "banco_blanco.svg");
                // Establecer el BindingContext para el formulario de Cuenta
                BindingContext = _viewModelCuenta;
                break;
        }
    }
    //Cuando el boton se presiona las interfaz del boton cambia de estilo
    private void EstilizarBotonSeleccionado(Button boton, string icono)
    {
        boton.BackgroundColor = Color.Parse("#4f46e5");
        boton.TextColor = Colors.White;
        boton.ImageSource = ImageSource.FromFile(icono);
    }

    private void ResetearEstilosBotones()
    {
        EstilizarBotonDesactivado(btnGasto, "cartera.svg");
        EstilizarBotonDesactivado(btnIngreso, "billete.svg");
        EstilizarBotonDesactivado(btnCuenta, "banco.svg");
    }
    //Cuando el boton no esta seleccionado se cambia el estilo
    private void EstilizarBotonDesactivado(Button boton, string icono)
    {
        boton.BackgroundColor = Color.Parse("#f3f4f6");
        boton.TextColor = Colors.Black;
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
        border.Stroke = Color.Parse("#4f46e5");
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

    private void entradaNombreCuenta_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderNombreCuenta);
    private void entradaNombreCuenta_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderNombreCuenta);

    private void entradaBancoCuenta_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderBancoCuenta);
    private void entradaBancoCuenta_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderBancoCuenta);

    private void entradaSaldoInicialCuenta_Focused(object sender, FocusEventArgs e) => OnEntryFocused(borderSaldoInicialCuenta);
    private void entradaSaldoInicialCuenta_Unfocused(object sender, FocusEventArgs e) => OnEntryUnfocused(borderSaldoInicialCuenta);

}
