using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Aplicacion.DTOs;

/// <summary>DTO para mostrar una transacción en listas</summary>
public record TransaccionResumenDto(
    int Id,
    int TarjetaId,
    string NombreTarjeta,
    string Descripcion,
    decimal Monto,
    TipoTransaccion Tipo,
    CategoriaTransaccion Categoria,
    bool CategoriaPredicha,
    float ConfianzaPrediccion,
    DateTime Fecha,
    string? Notas
);
