using SQLite;

namespace ClientesPagosApp.Models
{
    /*
    Descripción:
    Enumera los tipos de movimiento posibles sobre una cuenta.
    */
    public enum TipoMovimiento
    {
        Ingreso,
        Gasto
    }

    /*
    Descripción:
    Representa un movimiento (ingreso o gasto) dentro de una cuenta
    específica. El saldo actual de una cuenta se calcula sumando su
    SaldoInicial más todos sus movimientos.

    Entrada:
    No aplica (es una clase de datos).

    Salida:
    No aplica.

    Restricción:
    CuentaId debe corresponder a una cuenta existente.
    Monto debe ser mayor que cero (el signo lo determina el Tipo,
    no un monto negativo).
    */
    public class MovimientoCuenta
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int CuentaId { get; set; }

        public TipoMovimiento Tipo { get; set; }

        public decimal Monto { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public string Categoria { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        // Si este movimiento proviene de marcar un Pago como pagado,
        // guardamos aquí el Id de ese pago para trazabilidad.
        public int? PagoId { get; set; }
    }
}