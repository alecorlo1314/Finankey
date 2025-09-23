
using FinanKey.Presentacion.Intefaces;

namespace FinanKey.Presentacion.View.Validaciones;

public class IsNotNullOrEmptyRule<T> : IReglaValidacion<T>
{
    public string ValidandoMensaje { get; set; }
    public bool Revisar(T valor) => valor is string str && !string.IsNullOrWhiteSpace(str);
}

