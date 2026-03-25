using FinanzasApp.Domain.Enumeraciones;
using FinanzasApp.Domain.Interfaces;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace FinanzasApp.Infraestructura.Prediccion;

/// <summary>
/// Servicio de predicción de categorías usando modelos ONNX.
/// 
/// ARQUITECTURA DEL MODELO:
/// - Entrada: Texto de descripción de la transacción
/// - Preprocesamiento: TF-IDF simplificado con vocabulario fijo
/// - Salida: Vector de probabilidades por categoría (softmax)
/// 
/// MODELOS:
/// - gastos_model.onnx: Entrenado con descripciones de gastos
/// - ingresos_model.onnx: Entrenado con descripciones de ingresos
/// 
/// Los modelos se incluyen en el bundle de la app como EmbeddedResource.
/// </summary>
public class ServicioPrediccionOnnx : IServicioPrediccion, IAsyncDisposable
{
    // Umbral mínimo de confianza para considerar la predicción válida
    private const float UmbralConfianza = 0.60f;

    // Longitud máxima del vector de entrada (vocabulario)
    private const int TamanoVocabulario = 500;

    private InferenceSession? _sesionGastos;
    private InferenceSession? _sesionIngresos;

    public bool ModeloListo { get; private set; }

    /// <summary>
    /// Carga ambos modelos ONNX desde los recursos embebidos.
    /// Se ejecuta al inicio de la app para evitar latencia en la primera predicción.
    /// </summary>
    public async Task InicializarAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                // Los archivos .onnx se incluyen como MauiAsset en el proyecto
                var rutaGastos = Path.Combine(FileSystem.AppDataDirectory, "gastos_model.onnx");
                var rutaIngresos = Path.Combine(FileSystem.AppDataDirectory, "ingresos_model.onnx");

                // Copia los modelos desde el bundle de la app si no existen en AppData
                CopiarModeloSiNoExiste("gastos_model.onnx", rutaGastos);
                CopiarModeloSiNoExiste("ingresos_model.onnx", rutaIngresos);

                if (File.Exists(rutaGastos))
                    _sesionGastos = new InferenceSession(rutaGastos);

                if (File.Exists(rutaIngresos))
                    _sesionIngresos = new InferenceSession(rutaIngresos);
            });

            ModeloListo = _sesionGastos is not null && _sesionIngresos is not null;
        }
        catch (Exception ex)
        {
            // Si el modelo no carga, la app sigue funcionando sin predicción
            System.Diagnostics.Debug.WriteLine($"[ONNX] Error al cargar modelos: {ex.Message}");
            ModeloListo = false;
        }
    }

    public async Task<ResultadoPrediccion> PredecirCategoriaGastoAsync(string descripcion)
    {
        if (_sesionGastos is null || !ModeloListo)
            return PrediccionPorDefecto(CategoriaTransaccion.Otros);

        return await Task.Run(() => EjecutarPrediccion(_sesionGastos, descripcion, esGasto: true));
    }

    public async Task<ResultadoPrediccion> PredecirCategoriaIngresoAsync(string descripcion)
    {
        if (_sesionIngresos is null || !ModeloListo)
            return PrediccionPorDefecto(CategoriaTransaccion.SalarioSueldo);

        return await Task.Run(() => EjecutarPrediccion(_sesionIngresos, descripcion, esGasto: false));
    }

    /// <summary>
    /// Ejecuta la inferencia ONNX sobre la descripción dada.
    /// Preprocesa el texto, ejecuta el modelo y mapea la salida a una categoría.
    /// </summary>
    private ResultadoPrediccion EjecutarPrediccion(InferenceSession sesion, string descripcion, bool esGasto)
    {
        try
        {
            // 1. Preprocesamiento: convierte texto a vector numérico
            var vectorEntrada = ProcesarTexto(descripcion);

            // 2. Crea el tensor de entrada (shape: [1, TamanoVocabulario])
            var tensor = new DenseTensor<float>(vectorEntrada, [1, TamanoVocabulario]);
            var entradas = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input", tensor)
            };

            // 3. Inferencia
            using var salidas = sesion.Run(entradas);
            var probabilidades = salidas.First().AsEnumerable<float>().ToArray();

            // 4. Aplica softmax si el modelo no lo hace internamente
            var softmax = AplicarSoftmax(probabilidades);

            // 5. Determina categoría ganadora
            var indiceMayor = softmax.ToList().IndexOf(softmax.Max());
            var categoria = MapearIndiceACategoria(indiceMayor, esGasto);
            var confianza = softmax[indiceMayor];

            // 6. Construye mapa de todas las probabilidades para depuración
            var todasProbabilidades = new Dictionary<CategoriaTransaccion, float>();
            for (int i = 0; i < softmax.Length; i++)
            {
                var cat = MapearIndiceACategoria(i, esGasto);
                todasProbabilidades[cat] = softmax[i];
            }

            return new ResultadoPrediccion(
                CategoriaPredicha: categoria,
                Confianza: confianza,
                EsConfiable: confianza >= UmbralConfianza,
                TodasLasProbabilidades: todasProbabilidades
            );
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ONNX] Error en predicción: {ex.Message}");
            return PrediccionPorDefecto(esGasto ? CategoriaTransaccion.Otros : CategoriaTransaccion.SalarioSueldo);
        }
    }

    /// <summary>
    /// Convierte texto a vector de características (Bag of Words simplificado).
    /// En producción reemplazar por el mismo tokenizador usado en el entrenamiento.
    /// </summary>
    private static float[] ProcesarTexto(string texto)
    {
        var vector = new float[TamanoVocabulario];
        var palabras = texto.ToLowerInvariant()
            .Split([' ', ',', '.', '-', '_'], StringSplitOptions.RemoveEmptyEntries);

        foreach (var palabra in palabras)
        {
            // Hash simple para mapear palabras a índices del vocabulario
            var indice = Math.Abs(palabra.GetHashCode()) % TamanoVocabulario;
            vector[indice] += 1.0f;
        }

        // Normalización L2 para estabilidad numérica
        var magnitud = MathF.Sqrt(vector.Sum(v => v * v));
        if (magnitud > 0)
            for (int i = 0; i < vector.Length; i++)
                vector[i] /= magnitud;

        return vector;
    }

    /// <summary>Función Softmax para convertir logits en probabilidades (suma = 1)</summary>
    private static float[] AplicarSoftmax(float[] logits)
    {
        var max = logits.Max();
        var exp = logits.Select(l => MathF.Exp(l - max)).ToArray();
        var suma = exp.Sum();
        return exp.Select(e => e / suma).ToArray();
    }

    /// <summary>
    /// Mapea el índice de salida del modelo a la enumeración de categorías.
    /// El orden debe coincidir exactamente con las etiquetas usadas al entrenar.
    /// </summary>
    private static CategoriaTransaccion MapearIndiceACategoria(int indice, bool esGasto)
    {
        if (esGasto)
        {
            return indice switch
            {
                0 => CategoriaTransaccion.Alimentacion,
                1 => CategoriaTransaccion.Transporte,
                2 => CategoriaTransaccion.Entretenimiento,
                3 => CategoriaTransaccion.Salud,
                4 => CategoriaTransaccion.Educacion,
                5 => CategoriaTransaccion.Hogar,
                6 => CategoriaTransaccion.Ropa,
                7 => CategoriaTransaccion.Tecnologia,
                8 => CategoriaTransaccion.Viajes,
                9 => CategoriaTransaccion.Servicios,
                10 => CategoriaTransaccion.Restaurantes,
                11 => CategoriaTransaccion.Deportes,
                12 => CategoriaTransaccion.Suscripciones,
                _ => CategoriaTransaccion.Otros
            };
        }
        else
        {
            return indice switch
            {
                0 => CategoriaTransaccion.SalarioSueldo,
                1 => CategoriaTransaccion.Freelance,
                2 => CategoriaTransaccion.Inversiones,
                3 => CategoriaTransaccion.Reembolsos,
                _ => CategoriaTransaccion.Otros
            };
        }
    }

    /// <summary>Copia el modelo desde el bundle de la app a AppDataDirectory</summary>
    private static void CopiarModeloSiNoExiste(string nombreArchivo, string rutaDestino)
    {
        if (File.Exists(rutaDestino)) return;

        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync(nombreArchivo).Result;
            using var destino = File.Create(rutaDestino);
            stream.CopyTo(destino);
        }
        catch
        {
            // Si el archivo no está en el bundle, continúa sin modelo
        }
    }

    private static ResultadoPrediccion PrediccionPorDefecto(CategoriaTransaccion categoria) =>
        new(CategoriaPredicha: categoria, Confianza: 0f, EsConfiable: false);

    public async ValueTask DisposeAsync()
    {
        _sesionGastos?.Dispose();
        _sesionIngresos?.Dispose();
        await Task.CompletedTask;
    }
}
