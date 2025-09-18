using FinanKey.Presentacion.ViewModels;

namespace FinanKey.Presentacion.View;

public partial class DetalleTarjetaPage : ContentPage
{
    public DetalleTarjetaPage(ViewModelDetalleTarjeta detalleTarjeta)
    {
        InitializeComponent();
        //Inyeccion de dependencias del ViewModelDetalleTarjeta
        BindingContext = detalleTarjeta;
    }

    /// <summary>
    /// Metodo que se ejecuta al navegar a la pagina
    /// </summary>
    /// <param name="args"></param>
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
    }

    /// <summary>
    /// Contiene el nombre de los dos botones para la pestana de informacion y para movimientos
    /// </summary>
    private enum SeccionSeleccionada
    {
        Informacion,
        Movimientos
    }

    /// <summary>
    /// Este metodo recibe una seccion cuan se hace click en los botones de informacion o movimientos
    /// realiza todas las acciones de la UI, para el cambio de colores segun la seleccion
    /// </summary>
    /// <param name="seleccionada"></param>
    private void MostrarSeccion(SeccionSeleccionada seleccionada)
    {
        //Ponemos los dos botones en invisible
        SeccionInformacion.IsVisible = false;
        SeccionMovimientos.IsVisible = false;

        // Resetear estilos de los botones
        ResetearEstilosBotones();
        //Se realiza la accion correspondiente a cada boton segun su seleccion
        switch (seleccionada)
        {//Si selecciono seccion informacion
            case SeccionSeleccionada.Informacion:
                // Mostrar la seccion de informacion
                SeccionInformacion.IsVisible = true;
                // Estilizar el botón de informacion
                EstilizarBotonSeleccionado(BotonInformacion);
                break;
            //Si selecciono seccion movimientos
            case SeccionSeleccionada.Movimientos:
                // Mostrar la seccion de movimientos
                SeccionMovimientos.IsVisible = true;
                //Estilizar el botón de movimientos
                EstilizarBotonSeleccionado(BotonMovimientos);
                break;
        }
    }
    /// <summary>
    /// Metodo que resetea los estilos de los botones
    /// </summary>
    private void ResetearEstilosBotones()
    {
        EstilizarBotonDesactivado(BotonInformacion);

        EstilizarBotonDesactivado(BotonMovimientos);
    }
    /// <summary>
    /// Metodo que estiliza el boton que fue seleccionado 
    /// </summary>
    /// <param name="boton"></param>
    private void EstilizarBotonSeleccionado(Button boton)
    {
        boton.BackgroundColor = App.Current?.Resources["ColorAzulPricipal"] as Color;
        boton.TextColor = Colors.White;
    }
    /// <summary>
    /// Metodo que estiliza el boton que no fue seleccionado
    /// </summary>
    /// <param name="boton"></param>
    private void EstilizarBotonDesactivado(Button boton)
    {
        boton.BackgroundColor = App.Current?.Resources["ColorFondoComponentes"] as Color;
        boton.TextColor = App.Current?.Resources["ColorAzulPricipal"] as Color;
    }
    /// <summary>
    /// Evento que se ejecuta al hacer click en el boton de informacion
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BotonInformacion_Clicked(object sender, EventArgs e) => MostrarSeccion(SeccionSeleccionada.Informacion);
    /// <summary>
    /// Evento que se ejecuta al hacer click en el boton de movimientos
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void BotonMovimientos_Clicked(object sender, EventArgs e) => MostrarSeccion(SeccionSeleccionada.Movimientos);
}