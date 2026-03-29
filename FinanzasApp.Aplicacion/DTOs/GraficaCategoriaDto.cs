using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Aplicacion.DTOs;

/// <summary>
/// DTO para gráficas de distribución por categoría
/// </summary>
public record GraficaCategoriaDto(
    CategoriaTransaccion Categoria,
    decimal Monto,
    double Porcentaje,
    string ColorHex         
);
