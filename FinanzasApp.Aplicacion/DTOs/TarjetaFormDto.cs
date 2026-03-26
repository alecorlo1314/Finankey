using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Aplicacion.DTOs;

/// <summary>DTO para crear o editar una tarjeta</summary>
public record TarjetaFormDto(
    int? Id,                 
    string Nombre,
    string UltimosDigitos,
    TipoTarjeta Tipo,
    string ColorHex,
    string Banco,
    string RedTarjeta,
    decimal? LimiteCredito,
    decimal SaldoActual
);
