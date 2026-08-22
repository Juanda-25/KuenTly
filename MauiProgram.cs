using KuenTly.Data;
using KuenTly.Services.Implementations;
using KuenTly.Services.Interfaces;
using KuenTly.ViewModels.Clientes;
using KuenTly.ViewModels.Ventas;
using KuenTly.Views.Clientes;
using KuenTly.Views.Ventas;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KuenTly
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

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddDbContextFactory<KuenTlyDbContext>(options =>
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "kuently.db");
                options.UseSqlite($"Filename={dbPath}");
            });

            builder.Services.AddSingleton<IClienteService, ClienteService>();
            builder.Services.AddSingleton<IVentaService, VentaService>();

            builder.Services.AddTransient<ClientesViewModel>();
            builder.Services.AddTransient<ClientesPage>();
            builder.Services.AddTransient<ClienteFormViewModel>();
            builder.Services.AddTransient<ClienteFormPage>();
            builder.Services.AddTransient<ClienteDetalleViewModel>();
            builder.Services.AddTransient<ClienteDetallePage>();
            builder.Services.AddTransient<VentaFormViewModel>();
            builder.Services.AddTransient<VentaFormPage>();

            var app = builder.Build();

            var dbContextFactory = app.Services.GetRequiredService<IDbContextFactory<KuenTlyDbContext>>();
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                dbContext.Database.Migrate();
            }

            return app;
        }
    }
}