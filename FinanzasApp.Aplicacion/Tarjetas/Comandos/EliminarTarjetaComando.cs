using FinanzasApp.Aplicacion.Interfaces;

namespace FinanzasApp.Aplicacion.Tarjetas.Comandos;

/// <summary>Comando para eliminar lógicamente una tarjeta (la desactiva)</summary>
public record EliminarTarjetaComando(int TarjetaId) : IComando<bool>;
