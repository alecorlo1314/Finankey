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
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>FK hacia la tarjeta propietaria</summary>
    [Indexed]   // Índice en BD para acelerar consultas por tarjeta
    public int TarjetaId { get; set; }

    /// <summary>Descripción breve (ej: "Supermercado Walmart")</summary>
    public string Descripcion { get; set; } = string.Empty;

    /// <summary>Monto siempre positivo; el Tipo define si es gasto o ingreso</summary>
    public decimal Monto { get; set; }

    public TipoTransaccion Tipo { get; set; }

    /// <summary>Categoría asignada (por IA o manualmente)</summary>
    public CategoriaTransaccion Categoria { get; set; }

    /// <summary>true si la categoría fue sugerida por el modelo ONNX</summary>
    public bool CategoriaPredicha { get; set; }

    /// <summary>Nivel de confianza del modelo ONNX (0.0–1.0)</summary>
    public float ConfianzaPrediccion { get; set; }

    [Indexed]   // Índice para acelerar filtros y ordenamientos por fecha
    public DateTime Fecha { get; set; } = DateTime.Now;

    public string? Notas { get; set; }

    /// <summary>Navigation property — SQLite-net la ignora, se llena manualmente</summary>
    [Ignore]
    public Tarjeta? Tarjeta { get; set; }
}
