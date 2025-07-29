namespace FinanKey.View;

public partial class DetalleCuentaPage : ContentPage, IQueryAttributable
{
	public DetalleCuentaPage()
	{
		InitializeComponent();
	}
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Recibir parámetros
        if (query.TryGetValue("nombre", out var nombre))
        {
            labelNombre.Text = nombre.ToString();
        }

        if (query.TryGetValue("edad", out var edad))
        {
            labelEdad.Text = edad.ToString();
        }
    }
}