using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClientesPagosApp.Models;
using ClientesPagosApp.Services;

namespace ClientesPagosApp.ViewModels
{
    /*
    Descripción:
    ViewModel de la pantalla de clientes. Administra la lista de clientes
    visibles, el formulario de alta de un nuevo cliente y las acciones
    de guardar y activar/desactivar.

    Entrada:
    Recibe por inyección de dependencias una instancia de DatabaseService.

    Salida:
    Expone la colección ClientesLista para enlazar con la interfaz (XAML)
    y comandos (RelayCommand) para cargar y guardar clientes.

    Restricción:
    No permite guardar un cliente si el campo NuevoNombre está vacío.
    */
    public partial class ClientesViewModel : ObservableObject
    {
        private readonly DatabaseService _db;

        public ObservableCollection<Cliente> ClientesLista { get; } = new();

        [ObservableProperty]
        private string nuevoNombre = string.Empty;

        [ObservableProperty]
        private string nuevoTelefono = string.Empty;

        [ObservableProperty]
        private string nuevaNota = string.Empty;

        [ObservableProperty]
        private bool estaCargando;

        public ClientesViewModel(DatabaseService db)
        {
            _db = db;
        }

        /*
        Descripción:
        Carga (o recarga) la lista de clientes desde la base de datos
        y actualiza la colección observable que ve la interfaz.

        Entrada:
        Ninguna.

        Salida:
        Rellena ClientesLista con los clientes obtenidos.

        Restricción:
        Ninguna.
        */
        [RelayCommand]
        private async Task CargarClientesAsync()
        {
            if (EstaCargando)
                return;

            EstaCargando = true;

            var clientes = await _db.ObtenerClientesAsync();

            ClientesLista.Clear();
            foreach (var cliente in clientes)
                ClientesLista.Add(cliente);

            EstaCargando = false;
        }

        /*
        Descripción:
        Toma los datos del formulario (NuevoNombre, NuevoTelefono,
        NuevaNota), crea un Cliente nuevo, lo guarda en la base de datos
        y limpia el formulario para permitir cargar otro.

        Entrada:
        Usa las propiedades NuevoNombre, NuevoTelefono y NuevaNota.

        Salida:
        Inserta el cliente en la base de datos y lo agrega a ClientesLista.

        Restricción:
        NuevoNombre no puede estar vacío; si lo está, no hace nada
        (en una etapa posterior se puede mostrar un mensaje de error).
        */
        [RelayCommand]
        private async Task GuardarClienteAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevoNombre))
                return;

            var cliente = new Cliente
            {
                Nombre = NuevoNombre.Trim(),
                Telefono = string.IsNullOrWhiteSpace(NuevoTelefono) ? null : NuevoTelefono.Trim(),
                Nota = string.IsNullOrWhiteSpace(NuevaNota) ? null : NuevaNota.Trim()
            };

            await _db.GuardarClienteAsync(cliente);
            ClientesLista.Add(cliente);

            // Limpiar formulario
            NuevoNombre = string.Empty;
            NuevoTelefono = string.Empty;
            NuevaNota = string.Empty;
        }

        /*
        Descripción:
        Alterna el estado Activo/Inactivo de un cliente existente
        y guarda el cambio en la base de datos.

        Entrada:
        Recibe el objeto Cliente sobre el cual actuar.

        Salida:
        Actualiza el campo Activo del cliente en la base de datos.

        Restricción:
        El cliente debe ya existir (tener Id distinto de cero).
        */
        [RelayCommand]
        private async Task CambiarEstadoClienteAsync(Cliente cliente)
        {
            if (cliente is null || cliente.Id == 0)
                return;

            cliente.Activo = !cliente.Activo;
            await _db.GuardarClienteAsync(cliente);
        }
    }
}