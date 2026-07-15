protected override async void OnStart()
{
    var db = Handler?.MauiContext?.Services.GetService<DatabaseService>();
    if (db != null)
    {
        var cliente = new Cliente { Nombre = "Cliente de prueba", Telefono = "8888-8888" };
        await db.GuardarClienteAsync(cliente);
        var lista = await db.ObtenerClientesAsync();
        System.Diagnostics.Debug.WriteLine($"Clientes guardados: {lista.Count}");
    }
}