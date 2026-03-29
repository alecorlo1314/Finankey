using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Aplicacion.DTOs;

/// <summary>DTO para crear o editar una transacción</summary>
public record TransaccionFormDto(
    int? Id,
    int TarjetaId,
    string Descripcion,
    decimal Monto,
    TipoTransaccion Tipo,
    CategoriaTransaccion Categoria,
    bool CategoriaPredicha,
    float ConfianzaPrediccion,
    DateTime Fecha,
    string? Notas
);
