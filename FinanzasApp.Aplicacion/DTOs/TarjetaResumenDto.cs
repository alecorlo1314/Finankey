using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Aplicacion.DTOs;

/// <summary>DTO para mostrar el resumen de una tarjeta en listas y dashboard</summary>
public record TarjetaResumenDto(
    int Id,
    string Nombre,
    string UltimosDigitos,
    TipoTarjeta Tipo,
    string ColorHex,
    string Banco,
    string RedTarjeta,
    decimal SaldoActual,
    decimal? LimiteCredito,
    decimal CreditoDisponible,
    double PorcentajeUso,
    bool EstaActiva
);
