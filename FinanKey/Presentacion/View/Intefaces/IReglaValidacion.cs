
namespace FinanKey.Presentacion.Intefaces;

public interface IReglaValidacion<T> 
{
    string ValidandoMensaje { get; set; }
    bool Revisar(T valor);
}
