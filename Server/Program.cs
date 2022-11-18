using Microsoft.Extensions.Hosting;
using Orleans.Hosting;


Console.WriteLine("Silo starting... ");

var host = Host.CreateDefaultBuilder()
            .UseOrleans((ctx, silo) =>
            {
                silo.UseLocalhostClustering();
            })
            .Build();


await host.StartAsync();

Console.WriteLine("Press any key to exit...");
Console.ReadKey();

await host.StopAsync();