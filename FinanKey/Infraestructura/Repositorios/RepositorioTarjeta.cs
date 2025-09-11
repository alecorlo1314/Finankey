using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;

namespace FinanKey.Infraestructura.Repositorios
{
    public class RepositorioTarjeta : IServicioTarjeta
    {
        private readonly RepositorioBaseDatos _servicioBaseDatos;

        //Inyeccion de dependencias para el servicio de base de datos
        public RepositorioTarjeta(RepositorioBaseDatos servicioBaseDatos) => _servicioBaseDatos = servicioBaseDatos;

        /// <summary>
        /// Obtiene todas las tarjetas ordenadas por fecha de creación (más recientes primero)
        /// </summary>
        public async Task<List<Tarjeta>> ObtenerTodosAsync()
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.Table<Tarjeta>()
                                   .OrderByDescending(t => t.FechaCreacion)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todas las tarjetas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene una tarjeta por su ID
        /// </summary>
        public async Task<Tarjeta?> ObtenerPorIdAsync(int id)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.Table<Tarjeta>()
                                   .Where(t => t.Id == id)
                                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tarjeta por ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene tarjetas filtradas por tipo (Credito/Debito)
        /// </summary>
        public async Task<List<Tarjeta>> ObtenerPorTipoAsync(string tipo)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.Table<Tarjeta>()
                                   .Where(t => t.Tipo == tipo)
                                   .OrderByDescending(t => t.FechaCreacion)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tarjetas por tipo {tipo}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene tarjetas filtradas por banco
        /// </summary>
        public async Task<List<Tarjeta>> ObtenerPorBancoAsync(string banco)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.Table<Tarjeta>()
                                   .Where(t => t.Banco == banco)
                                   .OrderBy(t => t.Nombre)
                                   .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tarjetas por banco {banco}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Busca una tarjeta por los últimos 4 dígitos
        /// </summary>
        public async Task<Tarjeta?> ObtenerPorUltimos4DigitosAsync(string ultimos4Digitos)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.Table<Tarjeta>()
                                   .Where(t => t.Ultimos4Digitos == ultimos4Digitos)
                                   .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tarjeta por últimos 4 dígitos {ultimos4Digitos}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene tarjetas que vencen pronto (solo tarjetas de crédito)
        /// </summary>
        public async Task<List<Tarjeta>> ObtenerTarjetasVencenProntoAsync(int diasAnticipacion = 30)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                var fechaLimite = DateTime.UtcNow.AddDays(diasAnticipacion);

                // Obtener todas las tarjetas de crédito con vencimiento
                var tarjetas = await conexion.Table<Tarjeta>()
                                           .Where(t => t.Tipo == "Credito" && t.Vencimiento != null)
                                           .ToListAsync();

                // Filtrar por fecha de vencimiento en memoria
                var tarjetasVencenPronto = tarjetas.Where(t =>
                {
                    if (string.IsNullOrEmpty(t.Vencimiento) || t.Vencimiento.Length != 5)
                        return false;

                    var partes = t.Vencimiento.Split('/');
                    if (partes.Length != 2)
                        return false;

                    if (int.TryParse(partes[0], out int mes) && int.TryParse(partes[1], out int año))
                    {
                        // Convertir año de 2 dígitos a 4 dígitos
                        año = año < 50 ? 2000 + año : 1900 + año;

                        var fechaVencimiento = new DateTime(año, mes, DateTime.DaysInMonth(año, mes));
                        return fechaVencimiento <= fechaLimite && fechaVencimiento >= DateTime.UtcNow;
                    }
                    return false;
                }).ToList();

                return tarjetasVencenPronto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tarjetas que vencen pronto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Obtiene tarjetas ya vencidas (solo tarjetas de crédito)
        /// </summary>
        public async Task<List<Tarjeta>> ObtenerTarjetasVencidasAsync()
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();

                // Obtener todas las tarjetas de crédito con vencimiento
                var tarjetas = await conexion.Table<Tarjeta>()
                                           .Where(t => t.Tipo == "Credito" && t.Vencimiento != null)
                                           .ToListAsync();

                // Filtrar tarjetas vencidas en memoria
                var tarjetasVencidas = tarjetas.Where(t =>
                {
                    if (string.IsNullOrEmpty(t.Vencimiento) || t.Vencimiento.Length != 5)
                        return false;

                    var partes = t.Vencimiento.Split('/');
                    if (partes.Length != 2)
                        return false;

                    if (int.TryParse(partes[0], out int mes) && int.TryParse(partes[1], out int año))
                    {
                        // Convertir año de 2 dígitos a 4 dígitos
                        año = año < 50 ? 2000 + año : 1900 + año;

                        var fechaVencimiento = new DateTime(año, mes, DateTime.DaysInMonth(año, mes));
                        return fechaVencimiento < DateTime.UtcNow;
                    }
                    return false;
                }).ToList();

                return tarjetasVencidas;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener tarjetas vencidas: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Inserta una nueva tarjeta
        /// </summary>
        public async Task<int> InsertarAsync(Tarjeta tarjeta)
        {
            try
            {
                // Validar que no existe otra tarjeta con los mismos últimos 4 dígitos
                var existe = await ExisteTarjetaConUltimos4DigitosAsync(tarjeta.Ultimos4Digitos);
                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe una tarjeta con los últimos 4 dígitos '{tarjeta.Ultimos4Digitos}'");
                }

                // Validar formato de vencimiento si se proporciona
                if (!string.IsNullOrEmpty(tarjeta.Vencimiento))
                {
                    ValidarFormatoVencimiento(tarjeta.Vencimiento);
                }

                var conexion = await _servicioBaseDatos.ObtenerConexion();
                tarjeta.FechaCreacion = DateTime.UtcNow;
                return await conexion.InsertAsync(tarjeta);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al insertar tarjeta: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Actualiza una tarjeta existente
        /// </summary>
        public async Task<int> ActualizarAsync(Tarjeta tarjeta)
        {
            try
            {
                // Validar que no existe otra tarjeta con los mismos últimos 4 dígitos
                var existe = await ExisteTarjetaConUltimos4DigitosAsync(tarjeta.Ultimos4Digitos, tarjeta.Id);
                if (existe)
                {
                    throw new InvalidOperationException($"Ya existe otra tarjeta con los últimos 4 dígitos '{tarjeta.Ultimos4Digitos}'");
                }

                // Validar formato de vencimiento si se proporciona
                if (!string.IsNullOrEmpty(tarjeta.Vencimiento))
                {
                    ValidarFormatoVencimiento(tarjeta.Vencimiento);
                }

                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.UpdateAsync(tarjeta);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar tarjeta: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina una tarjeta por ID
        /// </summary>
        public async Task<int> EliminarAsync(int id)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.DeleteAsync<Tarjeta>(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar tarjeta con ID {id}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Elimina una tarjeta por objeto
        /// </summary>
        public async Task<int> EliminarAsync(Tarjeta tarjeta)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                return await conexion.DeleteAsync(tarjeta);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar tarjeta: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si existe una tarjeta con los últimos 4 dígitos especificados
        /// </summary>
        public async Task<bool> ExisteTarjetaConUltimos4DigitosAsync(string ultimos4Digitos)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                var count = await conexion.Table<Tarjeta>()
                                        .Where(t => t.Ultimos4Digitos == ultimos4Digitos)
                                        .CountAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar existencia de tarjeta con últimos 4 dígitos {ultimos4Digitos}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si existe una tarjeta con los últimos 4 dígitos especificados, excluyendo un ID específico
        /// </summary>
        public async Task<bool> ExisteTarjetaConUltimos4DigitosAsync(string ultimos4Digitos, int idExcluir)
        {
            try
            {
                var conexion = await _servicioBaseDatos.ObtenerConexion();
                var count = await conexion.Table<Tarjeta>()
                                        .Where(t => t.Ultimos4Digitos == ultimos4Digitos && t.Id != idExcluir)
                                        .CountAsync();
                return count > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar existencia de tarjeta con últimos 4 dígitos {ultimos4Digitos} excluyendo ID {idExcluir}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida el formato de vencimiento (MM/YY)
        /// </summary>
        private void ValidarFormatoVencimiento(string vencimiento)
        {
            if (string.IsNullOrEmpty(vencimiento) || vencimiento.Length != 5 || !vencimiento.Contains('/'))
            {
                throw new ArgumentException("El formato de vencimiento debe ser MM/YY");
            }

            var partes = vencimiento.Split('/');
            if (partes.Length != 2)
            {
                throw new ArgumentException("El formato de vencimiento debe ser MM/YY");
            }

            if (!int.TryParse(partes[0], out int mes) || mes < 1 || mes > 12)
            {
                throw new ArgumentException("El mes debe estar entre 01 y 12");
            }

            if (!int.TryParse(partes[1], out int año) || año < 0 || año > 99)
            {
                throw new ArgumentException("El año debe estar entre 00 y 99");
            }
        }
    }
}

// HELPER PARA MANEJO DE FECHAS DE VENCIMIENTO
public static class TarjetaHelper
{
    /// <summary>
    /// Convierte una fecha de vencimiento MM/YY a DateTime
    /// </summary>
    public static DateTime? ConvertirVencimientoADateTime(string? vencimiento)
    {
        if (string.IsNullOrEmpty(vencimiento) || vencimiento.Length != 5)
            return null;

        var partes = vencimiento.Split('/');
        if (partes.Length != 2)
            return null;

        if (int.TryParse(partes[0], out int mes) && int.TryParse(partes[1], out int año))
        {
            // Convertir año de 2 dígitos a 4 dígitos (asumiendo 2000-2099)
            año = año < 50 ? 2000 + año : 1900 + año;

            try
            {
                return new DateTime(año, mes, DateTime.DaysInMonth(año, mes));
            }
            catch
            {
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// Verifica si una tarjeta está vencida
    /// </summary>
    public static bool EstaVencida(string? vencimiento)
    {
        var fechaVencimiento = ConvertirVencimientoADateTime(vencimiento);
        return fechaVencimiento.HasValue && fechaVencimiento.Value < DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica si una tarjeta vence pronto
    /// </summary>
    public static bool VencePronto(string? vencimiento, int diasAnticipacion = 30)
    {
        var fechaVencimiento = ConvertirVencimientoADateTime(vencimiento);
        if (!fechaVencimiento.HasValue) return false;

        var fechaLimite = DateTime.UtcNow.AddDays(diasAnticipacion);
        return fechaVencimiento.Value <= fechaLimite && fechaVencimiento.Value >= DateTime.UtcNow;
    }

    /// <summary>
    /// Obtiene una representación amigable del estado de vencimiento
    /// </summary>
    public static string ObtenerEstadoVencimiento(string? vencimiento)
    {
        if (EstaVencida(vencimiento))
            return "Vencida";

        if (VencePronto(vencimiento, 30))
            return "Vence Pronto";

        return "Vigente";
    }
}