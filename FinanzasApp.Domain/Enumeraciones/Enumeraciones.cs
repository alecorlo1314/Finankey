namespace FinanzasApp.Domain.Enumeraciones;

/// <summary>Define si una tarjeta es de crédito o débito</summary>
public enum TipoTarjeta
{
    Credito = 0,
    Debito = 1
}

/// <summary>Define si una transacción representa un gasto o un ingreso</summary>
public enum TipoTransaccion
{
    Gasto = 0,
    Ingreso = 1
}

/// <summary>
/// Categorías de transacciones que el modelo ONNX puede predecir.
/// El orden numérico debe coincidir con las etiquetas del modelo entrenado.
/// </summary>
public enum CategoriaTransaccion
{
    Alimentacion = 0,
    Transporte = 1,
    Entretenimiento = 2,
    Salud = 3,
    Educacion = 4,
    Hogar = 5,
    Ropa = 6,
    Tecnologia = 7,
    Viajes = 8,
    Servicios = 9,    // Luz, agua, internet
    Restaurantes = 10,
    Deportes = 11,
    Suscripciones = 12,
    SalarioSueldo = 13,
    Freelance = 14,
    Inversiones = 15,
    Reembolsos = 16,
    Otros = 17
}
