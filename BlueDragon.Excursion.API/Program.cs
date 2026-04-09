using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BlueDragon.Excursion.API;

public class Program
{
    public const long MaxRequestBodySize = 100000000L;
    public static int Port { get; set; }

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        IConfiguration bindingConfig = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();

        Port = bindingConfig.GetValue<int?>("port") ?? 5000;

        return Host.CreateDefaultBuilder(args)
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
