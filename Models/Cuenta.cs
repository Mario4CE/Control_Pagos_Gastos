using SQLite;

namespace ClientesPagosApp.Models
{
    /*
    Descripción:
    Enumera los tipos de cuenta disponibles.
    */
    public enum TipoCuenta
    {
        Efectivo,
        Banco,
        Sinpe,
        Tarjeta,
        Otro
    }

    /*
    Descripción:
    Representa una cuenta de dinero (efectivo, banco, etc.) sobre la cual
    se registran movimientos de ingreso y gasto.

    Entrada:
    No aplica (es una clase de datos).

    Salida:
    No aplica.

    Restricción:
    El nombre de la cuenta no puede estar vacío.
    El saldo inicial no puede ser negativo.
    */
    public class Cuenta
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public TipoCuenta Tipo { get; set; } = TipoCuenta.Efectivo;

        public decimal SaldoInicial { get; set; }

        public bool Activa { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}