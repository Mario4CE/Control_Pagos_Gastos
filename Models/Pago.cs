using SQLite;

namespace ClientesPagosApp.Models
{
    /*
    Descripción:
    Enumera los métodos de pago disponibles para un Pago.
    */
    public enum MetodoPago
    {
        Efectivo,
        Sinpe,
        Transferencia,
        Tarjeta,
        Otro
    }

    /*
    Descripción:
    Enumera los estados posibles de un Pago.
    */
    public enum EstadoPago
    {
        Pendiente,
        Pagado
    }

    /*
    Descripción:
    Representa un pago asociado a un cliente. Guarda el monto, las fechas
    relevantes, el estado y el método utilizado.

    Entrada:
    No aplica (es una clase de datos).

    Salida:
    No aplica.

    Restricción:
    ClienteId debe corresponder a un cliente existente.
    Monto debe ser mayor que cero.
    FechaLimite no debe quedar vacía.
    */
    public class Pago
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int ClienteId { get; set; }

        public decimal Monto { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public DateTime FechaLimite { get; set; }

        public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;

        public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;

        public string? Nota { get; set; }

        // Si el pago ya se registró como ingreso en una cuenta,
        // guardamos aquí el Id de ese movimiento para no duplicarlo.
        public int? MovimientoCuentaId { get; set; }
    }
}