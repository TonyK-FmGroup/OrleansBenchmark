using Orleans;

namespace Grains;

[GenerateSerializer]
public class BranchStockInfo
{
    // [Id(0)] public Dictionary<string, StockInfo> Stock = new();
    // [Id(1)] public Dictionary<string, IStockGrain> StockGrains = new();
    [Id(0)] public Dictionary<string, StockInfo> BarcodeIndex = new();
}
