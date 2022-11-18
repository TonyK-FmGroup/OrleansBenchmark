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

// Set up a lot of stock grains all in the same branch
for (int i = 0; i < 1000; i++)
{
    var id = Guid.NewGuid();
    grainIds.Add(id);

    var grain = clusterClient.GetGrain<IStockGrain>(id);
    await grain.SetInfo(new StockInfo() { Id = id, Title = "Some product", Quantity = 1000, StoreId = "1", Barcode = id.ToString() });
}

Console.WriteLine($"Added items in {stopwatch.ElapsedMilliseconds} ms");

var branchStockGrain = clusterClient.GetGrain<IBranchStockGrain>("1");

stopwatch.Restart();

// Get all the stock items in this branch, from the copied version of the data
List<StockInfo> stockitems = new();
for (int i = 0; i < 100; i++)
{
    stockitems = await branchStockGrain.GetStock();
}
Console.WriteLine($"Retrieved items from cache in {stopwatch.ElapsedMilliseconds} ms for {stockitems.Count}");


stopwatch.Restart();

// Get all the stock items in the branch, asking each grain for the data 
//for (int i = 0; i < 100; i++)
//{
//    stockitems = await branchStockGrain.GetStockFromGrains();
//}
//Console.WriteLine($"Retrieved items from grains in {stopwatch.ElapsedMilliseconds} ms for {stockitems.Count}");

StockInfo stockitem;
//foreach (var item in grainIds)
//{
//    stockitem = await branchStockGrain.GetByBarcode(item.ToString());
//}

//Console.WriteLine($"Retrieved items by barcode {stopwatch.ElapsedMilliseconds} ms");


stopwatch.Restart();
foreach (var item in grainIds)
{
    stockitem = await branchStockGrain.GetByBarcodeIndex(item.ToString());
}
Console.WriteLine($"Retrieved items by barcode index {stopwatch.ElapsedMilliseconds} ms");


Console.WriteLine("Process complete.");

Console.ReadLine();