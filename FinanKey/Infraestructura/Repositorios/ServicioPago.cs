using FinanKey.Dominio.Models;
using FinanKey.Dominio.Interfaces;

namespace FinanKey.Infraestructura.Repositorios
{
    //internal class ServicioPago : IServicioPago
    //{
    //    private readonly ServicioBaseDatos _servicioBaseDatos;
    //    //Inyeccion de dependencias para el servicio de base de datos
    //    public ServicioPago(ServicioBaseDatos servicioBaseDatos) => _servicioBaseDatos = servicioBaseDatos;

    //    //public async Task PagoCreditoGastoAsync(int expenseMovementId, int payerCardId, decimal amount, bool allowNegative = false)
    //    //{
    //    //    await _servicioBaseDatos.CorrerEnTransicionAsync(async c =>
    //    //    {
    //    //        var gasto = await c.FindAsync<Movimiento>(expenseMovementId)
    //    //                    ?? throw new InvalidOperationException("Movimiento no encontrado.");
    //    //        if (gasto.Tipo != "Gasto" || gasto.TarjetaId == null)
    //    //            throw new InvalidOperationException("El movimiento no es un gasto con tarjeta.");

    //    //        var credito = await c.FindAsync<Tarjeta>(gasto.TarjetaId.Value)
    //    //                      ?? throw new InvalidOperationException("Tarjeta de crédito no encontrada.");
    //    //        if (credito.Tipo != "Credito")
    //    //            throw new InvalidOperationException("El movimiento no pertenece a una tarjeta de crédito.");

    //    //        var pagadora = await c.FindAsync<Tarjeta>(payerCardId)
    //    //                      ?? throw new InvalidOperationException("Tarjeta de pago no encontrada.");
    //    //        if (pagadora.Tipo == "Credito")
    //    //            throw new InvalidOperationException("La tarjeta de pago debe ser Débito o Corriente.");

    //    //        var payAmount = Math.Min(Math.Abs(amount), credito.SaldoPendiente);
    //    //        if (payAmount <= 0) throw new InvalidOperationException("Monto de pago inválido.");

    //    //        // Check negative balance
    //    //        if (!allowNegative && (pagadora.SaldoActual - payAmount) < 0)
    //    //            throw new InvalidOperationException("Saldo insuficiente en la cuenta de pago.");

    //    //        // Create payment movement (Gasto) on payer card
    //    //        var pago = new Movimiento
    //    //        {
    //    //            Tipo = "Gasto",
    //    //            Monto = payAmount,
    //    //            Fecha = DateTime.UtcNow,
    //    //            TarjetaId = pagadora.Id,
    //    //            Comercio = $"Pago TC • {credito.Alias} • •••• {credito.UltimosCuatroDigitos}",
    //    //            EsPagado = null
    //    //        };
    //    //        await c.InsertAsync(pago);

    //    //        // Update payer balances and credit card
    //    //        pagadora.SaldoActual -= payAmount;
    //    //        await c.UpdateAsync(pagadora);

    //    //        credito.SaldoPendiente -= payAmount;
    //    //        if (credito.SaldoPendiente < 0) credito.SaldoPendiente = 0;
    //    //        await c.UpdateAsync(credito);

    //    //        // Mark original expense as paid
    //    //        gasto.EsPagado = true;
    //    //        gasto.MovimientoPagoId = pago.Id;
    //    //        await c.UpdateAsync(gasto);
    //    //    });
    //    //}

    //    //public async Task PagoMuchosCreditoGastoAsync(IEnumerable<int> expenseIds, int payerCardId, bool allowNegative = false)
    //    //{
    //    //    // Sum amounts and pay one by one atomically
    //    //    await _servicioBaseDatos.CorrerEnTransicionAsync(async c =>
    //    //    {
    //    //        var pagadora = await c.FindAsync<Tarjeta>(payerCardId)
    //    //                      ?? throw new InvalidOperationException("Tarjeta de pago no encontrada.");
    //    //        if (pagadora.Tipo == "Credito")
    //    //            throw new InvalidOperationException("La tarjeta de pago debe ser Débito o Corriente.");

    //    //        decimal total = 0m;
    //    //        var gastos = new List<Movimiento>();
    //    //        foreach (var id in expenseIds)
    //    //        {
    //    //            var g = await c.FindAsync<Movimiento>(id);
    //    //            if (g == null) continue;
    //    //            var card = g.TarjetaId.HasValue ? await c.FindAsync<Tarjeta>(g.TarjetaId.Value) : null;
    //    //            if (g.Tipo == "Gasto" && card?.Tipo == "Credito" && (g.EsPagado ?? false) == false)
    //    //            {
    //    //                total += Math.Abs(g.Monto);
    //    //                gastos.Add(g);
    //    //            }
    //    //        }

    //    //        if (!allowNegative && (pagadora.SaldoActual - total) < 0)
    //    //            throw new InvalidOperationException("Saldo insuficiente para pagar todos los movimientos seleccionados.");

    //    //        // Execute individual payments
    //    //        foreach (var g in gastos)
    //    //        {
    //    //            var tc = await c.FindAsync<Tarjeta>(g.TarjetaId!.Value);
    //    //            var pago = new Movimiento
    //    //            {
    //    //                Tipo = "Gasto",
    //    //                Monto = Math.Abs(g.Monto),
    //    //                Fecha = DateTime.UtcNow,
    //    //                TarjetaId = pagadora.Id,
    //    //                Comercio = $"Pago TC • {tc!.Alias} • •••• {tc.UltimosCuatroDigitos}",
    //    //                EsPagado = null
    //    //            };
    //    //            await c.InsertAsync(pago);

    //    //            pagadora.SaldoActual -= Math.Abs(g.Monto);
    //    //            await c.UpdateAsync(pagadora);

    //    //            tc.SaldoPendiente -= Math.Abs(g.Monto);
    //    //            if (tc.SaldoPendiente < 0) tc.SaldoPendiente = 0;
    //    //            await c.UpdateAsync(tc);

    //    //            g.EsPagado = true;
    //    //            g.MovimientoPagoId = pago.Id;
    //    //            await c.UpdateAsync(g);
    //    //        }
    //    //    });
    //    //}
    //}
}
