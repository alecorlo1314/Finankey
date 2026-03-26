using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Aplicacion.DTOs;

/// <summary>
/// DTO para crear o editar una tarjeta
/// </summary>
public record TarjetaFormDto(
    int? Id,                 
    string Nombre,
    string UltimosDigitos,
    TipoTarjeta Tipo,       // (debito o credito)
    string ColorHex,
    string Banco,
    string RedTarjeta,
    decimal? LimiteCredito,  // (solo debito)
    decimal SaldoActual,
    int MesVencimiento,      
    int AnioVencimiento,     
    int? DiaCorte,           // (solo crédito)
    int? DiaPago             // (solo crédito)
);
