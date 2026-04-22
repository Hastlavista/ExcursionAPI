using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BlueDragon.Excursion.API;

public class Program
{
    public const long MaxRequestBodySize = 100000000L;
    public static int Port { get; set; }

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341")
            .CreateBootstrapLogger();

        try
        {
            IHostBuilder hostBuilder = CreateHostBuilder(args);
            Log.Information("Starting BlueDragon.Excursion.API on port {Port}", Port);
            hostBuilder.Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        IConfiguration bindingConfig = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        Port = bindingConfig.GetValue<int?>("port") ?? 5000;

        return Host.CreateDefaultBuilder(args)
            .UseSerilog((context, services, configuration) => configuration
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.Seq("http://localhost:5341"))
            .UseDefaultServiceProvider(opts => opts.ValidateScopes = false)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseKestrel(options =>
                {
                    options.Limits.MaxRequestBodySize = MaxRequestBodySize;
                    options.Listen(IPAddress.Any, Port);
                });
            });
    }
}
