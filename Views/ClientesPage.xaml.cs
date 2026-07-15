using ClientesPagosApp.ViewModels;

namespace ClientesPagosApp.Views
{
    /*
    Descripción:
    Code-behind de la página de clientes. Su única responsabilidad es
    inicializar el componente visual e indicar al ViewModel que cargue
    los datos cada vez que la página aparece en pantalla.

    Entrada:
    Recibe por inyección de dependencias el ClientesViewModel.

    Salida:
    Ninguna directa; delega toda la lógica al ViewModel.

    Restricción:
    Ninguna.
    */
    public partial class ClientesPage : ContentPage
    {
        private readonly ClientesViewModel _viewModel;

        public ClientesPage(ClientesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarClientesCommand.ExecuteAsync(null);
        }
    }
}