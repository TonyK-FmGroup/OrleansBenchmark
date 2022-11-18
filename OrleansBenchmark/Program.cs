using Grains;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Hosting;
using System.Diagnostics;

Console.WriteLine("Starting stock simulator...");

var host = Host.CreateDefaultBuilder()
                .UseOrleansClient((ctx, client) =>
                {
                    client.UseLocalhostClustering();

                })
                .Build();
host.Start();

var clusterClient = host.Services.GetRequiredService<IClusterClient>();


var stopwatch = new Stopwatch();

List<Guid> grainIds = new();

stopwatch.Start();
for (int i = 0; i < 1000; i++)
{
    var id = Guid.NewGuid();
    grainIds.Add(id);

    var grain = clusterClient.GetGrain<IStockGrain>(id);
    await grain.SetInfo(new StockInfo() { Id = id, Title = "Some product", Quantity = 1000, StoreId = "1" });
}

Console.WriteLine($"Added items in {stopwatch.ElapsedMilliseconds} ms");

var branchStockGrain = clusterClient.GetGrain<IBranchStockGrain>("1");

stopwatch.Restart();


Dictionary<string, StockInfo> stockitems = new();
for (int i = 0; i < 100; i++)
{
    stockitems = await branchStockGrain.GetStock();
}

Console.WriteLine($"Retrieved items from cache in {stopwatch.ElapsedMilliseconds} ms for {stockitems.Count}");




stopwatch.Restart();

for (int i = 0; i < 100; i++)
{
    stockitems = await branchStockGrain.GetStockFromGrains();
}

Console.WriteLine($"Retrieved items from grains in {stopwatch.ElapsedMilliseconds} ms for {stockitems.Count}");

Console.WriteLine("Process complete.");

Console.ReadLine();