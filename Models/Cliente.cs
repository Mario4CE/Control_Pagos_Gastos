using SQLite;

namespace ClientesPagosApp.Models
{
    /*
    Descripción:
    Representa a un cliente registrado en la aplicación. Es la entidad
    principal a la que se asocian los pagos.

    Entrada:
    No aplica (es una clase de datos, no un método).

    Salida:
    No aplica.

    Restricción:
    El nombre no puede estar vacío.
    El campo Activo indica si el cliente sigue vigente o fue dado de baja
    (no se elimina físicamente para no perder el historial de pagos).
    */
    public class Cliente
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Telefono { get; set; }

        public string? Nota { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}