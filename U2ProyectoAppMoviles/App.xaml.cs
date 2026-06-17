using Microsoft.Extensions.DependencyInjection;

namespace U2ProyectoAppMoviles
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
          
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}