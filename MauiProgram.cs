using ClientesPagosApp.Services;
using ClientesPagosApp.ViewModels;
using ClientesPagosApp.Views;

namespace ClientesPagosApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Registramos el servicio de base de datos como Singleton:
            // una sola instancia compartida en toda la app.
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<ClientesViewModel>();
            builder.Services.AddTransient<ClientesPage>();

            return builder.Build();
        }
    }
}