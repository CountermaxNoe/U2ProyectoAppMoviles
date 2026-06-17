using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using U2ProyectoAppMoviles.Services;
using U2ProyectoAppMoviles.ViewModels;
using U2ProyectoAppMoviles.Views;


namespace U2ProyectoAppMoviles
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://crudeness-stinging-thirty.ngrok-free.dev/")
            });

            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<IncidenteService>();
            builder.Services.AddSingleton<ConnectivityService>();

            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddTransient<NuevaDenunciaViewModel>();
            builder.Services.AddTransient<DetalleIncidenteViewModel>();

            builder.Services.AddSingleton<SplashPage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddTransient<NuevaDenunciaPage>();
            builder.Services.AddTransient<DetalleIncidentePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
