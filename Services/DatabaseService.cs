using SQLite;
using ClientesPagosApp.Models;

namespace ClientesPagosApp.Services
{
    /*
    Descripción:
    Servicio central de acceso a datos. Administra la conexión a la base
    de datos SQLite local y expone métodos para crear, leer, actualizar
    y eliminar Clientes, Pagos, Cuentas y Movimientos de cuenta.

    Entrada:
    No recibe parámetros al construirse; internamente resuelve la ruta
    del archivo de base de datos dentro del almacenamiento de la app.

    Salida:
    Expone métodos async que devuelven listas, entidades individuales
    o un entero que indica el resultado de la operación (filas afectadas).

    Restricción:
    Debe existir una única instancia de este servicio en toda la app
    (se registra como Singleton en MauiProgram.cs) para evitar abrir
    múltiples conexiones al mismo archivo de base de datos.
    */
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _conexion;

        /*
        Descripción:
        Inicializa la conexión a la base de datos y crea las tablas
        si no existen todavía. Es seguro llamarlo varias veces:
        si ya está inicializada, no hace nada.

        Entrada:
        Ninguna.

        Salida:
        Deja lista la propiedad interna _conexion para ser usada
        por el resto de los métodos del servicio.

        Restricción:
        Debe ejecutarse (await) antes de usar cualquier otro método
        de este servicio.
        */
        private async Task InicializarAsync()
        {
            if (_conexion is not null)
                return;

            var rutaBaseDatos = Path.Combine(
                FileSystem.AppDataDirectory, "clientespagos.db3");

            _conexion = new SQLiteAsyncConnection(rutaBaseDatos);

            await _conexion.CreateTableAsync<Cliente>();
            await _conexion.CreateTableAsync<Pago>();
            await _conexion.CreateTableAsync<Cuenta>();
            await _conexion.CreateTableAsync<MovimientoCuenta>();
        }

        // ---------------- CLIENTES ----------------

        /*
        Descripción:
        Obtiene la lista completa de clientes registrados.

        Entrada:
        Ninguna.

        Salida:
        Lista de objetos Cliente ordenados por nombre.

        Restricción:
        Ninguna.
        */
        public async Task<List<Cliente>> ObtenerClientesAsync()
        {
            await InicializarAsync();
            return await _conexion!.Table<Cliente>()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        /*
        Descripción:
        Guarda un cliente nuevo o actualiza uno existente, según si
        ya tiene Id asignado.

        Entrada:
        Recibe un objeto Cliente con al menos el Nombre completado.

        Salida:
        Devuelve el número de filas afectadas (1 si tuvo éxito).

        Restricción:
        El nombre del cliente no puede estar vacío.
        */
        public async Task<int> GuardarClienteAsync(Cliente cliente)
        {
            await InicializarAsync();

            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new ArgumentException("El nombre del cliente no puede estar vacío.");

            if (cliente.Id != 0)
                return await _conexion!.UpdateAsync(cliente);

            return await _conexion!.InsertAsync(cliente);
        }

        /*
        Descripción:
        Elimina un cliente de la base de datos.

        Entrada:
        Recibe el objeto Cliente a eliminar.

        Salida:
        Devuelve el número de filas afectadas.

        Restricción:
        En esta etapa no se valida si el cliente tiene pagos asociados;
        se recomienda usar el campo Activo en vez de eliminar clientes
        con historial.
        */
        public async Task<int> EliminarClienteAsync(Cliente cliente)
        {
            await InicializarAsync();
            return await _conexion!.DeleteAsync(cliente);
        }

        // ---------------- PAGOS ----------------

        /*
        Descripción:
        Obtiene la lista completa de pagos registrados, sin filtrar.

        Entrada:
        Ninguna.

        Salida:
        Lista de objetos Pago ordenados por fecha límite ascendente.

        Restricción:
        Ninguna.
        */
        public async Task<List<Pago>> ObtenerPagosAsync()
        {
            await InicializarAsync();
            return await _conexion!.Table<Pago>()
                .OrderBy(p => p.FechaLimite)
                .ToListAsync();
        }

        /*
        Descripción:
        Obtiene todos los pagos asociados a un cliente específico.

        Entrada:
        Recibe el Id del cliente.

        Salida:
        Lista de objetos Pago que pertenecen a ese cliente.

        Restricción:
        Si el cliente no existe, devuelve una lista vacía (no lanza error).
        */
        public async Task<List<Pago>> ObtenerPagosPorClienteAsync(int clienteId)
        {
            await InicializarAsync();
            return await _conexion!.Table<Pago>()
                .Where(p => p.ClienteId == clienteId)
                .OrderBy(p => p.FechaLimite)
                .ToListAsync();
        }

        /*
        Descripción:
        Guarda un pago nuevo o actualiza uno existente.

        Entrada:
        Recibe un objeto Pago con Cliente, Monto, FechaLimite y Metodo
        completados.

        Salida:
        Devuelve el número de filas afectadas.

        Restricción:
        El monto debe ser mayor que cero.
        El ClienteId debe corresponder a un cliente existente.
        */
        public async Task<int> GuardarPagoAsync(Pago pago)
        {
            await InicializarAsync();

            if (pago.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor que cero.");

            var cliente = await _conexion!.Table<Cliente>()
                .Where(c => c.Id == pago.ClienteId)
                .FirstOrDefaultAsync();

            if (cliente is null)
                throw new ArgumentException("El cliente asociado no existe.");

            if (pago.Id != 0)
                return await _conexion!.UpdateAsync(pago);

            return await _conexion!.InsertAsync(pago);
        }

        /*
        Descripción:
        Elimina un pago de la base de datos.

        Entrada:
        Recibe el objeto Pago a eliminar.

        Salida:
        Devuelve el número de filas afectadas.

        Restricción:
        Ninguna adicional en esta etapa.
        */
        public async Task<int> EliminarPagoAsync(Pago pago)
        {
            await InicializarAsync();
            return await _conexion!.DeleteAsync(pago);
        }

        // ---------------- CUENTAS ----------------

        /*
        Descripción:
        Obtiene la lista completa de cuentas registradas.

        Entrada:
        Ninguna.

        Salida:
        Lista de objetos Cuenta ordenados por nombre.

        Restricción:
        Ninguna.
        */
        public async Task<List<Cuenta>> ObtenerCuentasAsync()
        {
            await InicializarAsync();
            return await _conexion!.Table<Cuenta>()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        /*
        Descripción:
        Guarda una cuenta nueva o actualiza una existente.

        Entrada:
        Recibe un objeto Cuenta con Nombre y SaldoInicial completados.

        Salida:
        Devuelve el número de filas afectadas.

        Restricción:
        El nombre no puede estar vacío.
        El saldo inicial no puede ser negativo.
        */
        public async Task<int> GuardarCuentaAsync(Cuenta cuenta)
        {
            await InicializarAsync();

            if (string.IsNullOrWhiteSpace(cuenta.Nombre))
                throw new ArgumentException("El nombre de la cuenta no puede estar vacío.");

            if (cuenta.SaldoInicial < 0)
                throw new ArgumentException("El saldo inicial no puede ser negativo.");

            if (cuenta.Id != 0)
                return await _conexion!.UpdateAsync(cuenta);

            return await _conexion!.InsertAsync(cuenta);
        }

        /*
        Descripción:
        Elimina una cuenta de la base de datos.

        Entrada:
        Recibe el objeto Cuenta a eliminar.

        Salida:
        Devuelve el número de filas afectadas.

        Restricción:
        En esta etapa no se valida si tiene movimientos asociados.
        */
        public async Task<int> EliminarCuentaAsync(Cuenta cuenta)
        {
            await InicializarAsync();
            return await _conexion!.DeleteAsync(cuenta);
        }

        // ---------------- MOVIMIENTOS DE CUENTA ----------------

        /*
        Descripción:
        Obtiene todos los movimientos asociados a una cuenta específica.

        Entrada:
        Recibe el Id de la cuenta.

        Salida:
        Lista de objetos MovimientoCuenta ordenados por fecha descendente
        (los más recientes primero).

        Restricción:
        Ninguna.
        */
        public async Task<List<MovimientoCuenta>> ObtenerMovimientosPorCuentaAsync(int cuentaId)
        {
            await InicializarAsync();
            return await _conexion!.Table<MovimientoCuenta>()
                .Where(m => m.CuentaId == cuentaId)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        /*
        Descripción:
        Guarda un movimiento nuevo o actualiza uno existente.

        Entrada:
        Recibe un objeto MovimientoCuenta con CuentaId, Tipo, Monto
        y Categoria completados.

        Salida:
        Devuelve el número de filas afectadas.

        Restricción:
        El monto debe ser mayor que cero.
        La cuenta asociada debe existir.
        */
        public async Task<int> GuardarMovimientoAsync(MovimientoCuenta movimiento)
        {
            await InicializarAsync();

            if (movimiento.Monto <= 0)
                throw new ArgumentException("El monto debe ser mayor que cero.");

            var cuenta = await _conexion!.Table<Cuenta>()
                .Where(c => c.Id == movimiento.CuentaId)
                .FirstOrDefaultAsync();

            if (cuenta is null)
                throw new ArgumentException("La cuenta asociada no existe.");

            if (movimiento.Id != 0)
                return await _conexion!.UpdateAsync(movimiento);

            return await _conexion!.InsertAsync(movimiento);
        }

        /*
        Descripción:
        Calcula el saldo actual de una cuenta sumando su saldo inicial
        con todos sus movimientos (ingresos suman, gastos restan).

        Entrada:
        Recibe el Id de la cuenta.

        Salida:
        Devuelve el saldo actual como decimal.

        Restricción:
        Si la cuenta no existe, devuelve 0.
        */
        public async Task<decimal> CalcularSaldoCuentaAsync(int cuentaId)
        {
            await InicializarAsync();

            var cuenta = await _conexion!.Table<Cuenta>()
                .Where(c => c.Id == cuentaId)
                .FirstOrDefaultAsync();

            if (cuenta is null)
                return 0;

            var movimientos = await ObtenerMovimientosPorCuentaAsync(cuentaId);

            decimal saldo = cuenta.SaldoInicial;
            foreach (var mov in movimientos)
            {
                saldo += mov.Tipo == TipoMovimiento.Ingreso ? mov.Monto : -mov.Monto;
            }

            return saldo;
        }
    }
}