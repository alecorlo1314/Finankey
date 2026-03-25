using FinanzasApp.Domain.Enumeraciones;

namespace FinanzasApp.Domain.Interfaces;

/// <summary>
/// Contrato para el servicio de predicción de categorías usando modelos ONNX.
/// Predice la categoría más probable para una transacción según su descripción.
/// </summary>
public interface IServicioPrediccion
{
    /// <summary>
    /// Predice la categoría de un gasto basándose en la descripción textual.
    /// Ejemplo: "Walmart Supermercado" → CategoriaTransaccion.Alimentacion
    /// </summary>
    Task<ResultadoPrediccion> PredecirCategoriaGastoAsync(string descripcion);

    /// <summary>
    /// Predice la categoría de un ingreso basándose en la descripción textual.
    /// Ejemplo: "Transferencia empresa XYZ" → CategoriaTransaccion.SalarioSueldo
    /// </summary>
    Task<ResultadoPrediccion> PredecirCategoriaIngresoAsync(string descripcion);

    /// <summary>Indica si el modelo ONNX está cargado y listo para predicciones</summary>
    bool ModeloListo { get; }

    /// <summary>Carga el modelo ONNX en memoria (llamar al inicio de la app)</summary>
    Task InicializarAsync();
}

/// <summary>Resultado de una predicción del modelo ONNX</summary>
public record ResultadoPrediccion(
    CategoriaTransaccion CategoriaPredicha,
    float Confianza,            // Valor entre 0.0 y 1.0
    bool EsConfiable,           // true si Confianza >= umbral mínimo (ej: 0.65)
    Dictionary<CategoriaTransaccion, float>? TodasLasProbabilidades = null
);
