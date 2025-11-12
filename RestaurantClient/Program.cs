using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace RestaurantClient;

internal class Program
{
    static async Task Main()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Build())
            .Enrich.FromLogContext()
            .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "logs", $"test-sms-console-app-{DateTime.Now:yyyyMMdd}.log"))
            .CreateLogger();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<OrderCreator>();
            })
            .UseSerilog()
            .Build();

        var ocs = ActivatorUtilities.CreateInstance<OrderCreator>(host.Services);
        await ocs.Run();
    }
}
