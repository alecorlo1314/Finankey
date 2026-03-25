using FinanzasApp.Domain.Enumeraciones;
using SQLite;

namespace FinanzasApp.Domain.Entidades;

/// <summary>
/// Entidad que representa una transacción financiera (gasto o ingreso).
/// Siempre vinculada a una tarjeta mediante TarjetaId (FK).
/// </summary>
[Table("Transaccion")]
public class Transaccion
{
    // ── Clave primaria ───────────────────────────────
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // ── Relación con tarjeta ─────────────────────────
    /// FK hacia la tarjeta propietaria
    [Indexed]   // índice en BD para acelerar consultas por tarjeta
    public int TarjetaId { get; set; }

    // ── Datos principales ────────────────────────────
    /// Descripción breve (ej: "Supermercado Walmart")
    public string Descripcion { get; set; } = string.Empty;

    /// Monto siempre positivo; el Tipo define si es gasto o ingreso
    public decimal Monto { get; set; }

    /// Tipo de transacción: Gasto o Ingreso
    public TipoTransaccion Tipo { get; set; }

    // ── Clasificación ────────────────────────────────
    /// Categoría asignada (por IA o manualmente)
    public CategoriaTransaccion Categoria { get; set; }

    /// true si la categoría fue sugerida por el modelo ONNX
    public bool CategoriaPredicha { get; set; }

    /// Nivel de confianza del modelo ONNX (0.0–1.0)
    public float ConfianzaPrediccion { get; set; }

    // ── Metadatos ────────────────────────────────────
    /// Fecha de la transacción
    [Indexed]   // índice para acelerar filtros y ordenamientos por fecha
    public DateTime Fecha { get; set; } = DateTime.Now;

    /// Notas adicionales opcionales
    public string? Notas { get; set; }

    // ── Propiedades de navegación (no se guardan en BD) ─
    /// Relación con la tarjeta (se llena manualmente)
    [Ignore]
    public Tarjeta? Tarjeta { get; set; }
}
