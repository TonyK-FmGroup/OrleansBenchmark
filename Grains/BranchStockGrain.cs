using Orleans;

namespace Grains;

public interface IBranchStockGrain : IGrainWithStringKey
{
    Task ReceiveStock(string Id, StockInfo stockItem, IStockGrain stockGrain);

    Task<Dictionary<string, StockInfo>> GetStock();

    Task<Dictionary<string, StockInfo>> GetStockFromGrains();

    Task<StockInfo> GetByBarcode(string barcode);

    Task<StockInfo> GetByBarcodeIndex(string barcode);
}

/// <summary>
/// Maintains a collection of stock that belongs to a given store
/// The grain ID is the store ID.
/// </summary>
public class BranchStockGrain : IBranchStockGrain
{
    private readonly BranchStockInfo State = new();

    public Task<StockInfo> GetByBarcode(string barcode)
    {
        var stock = State.Stock.FirstOrDefault(x => x.Value.Barcode == barcode).Value;
        return Task.FromResult(stock);
    }

    public Task<StockInfo> GetByBarcodeIndex(string barcode)
    {
        var stock = State.BarcodeIndex[barcode];
        return Task.FromResult(stock);
    }

    public Task<Dictionary<string, StockInfo>> GetStock()
    {
        return Task.FromResult(State.Stock);
    }

    public Task<Dictionary<string, StockInfo>> GetStockFromGrains()
    {
        var info = new Dictionary<string, StockInfo>();
        info = State.StockGrains.ToDictionary(g => g.Key, s => s.Value.GetStockInfo().Result);
        return Task.FromResult(State.Stock);
    }

    public Task ReceiveStock(string Id, StockInfo stockInfo, IStockGrain stockGrain)
    {
        if (State.Stock.ContainsKey(Id) == false)
        {
            State.Stock.Add(Id, stockInfo);
        }

        if (State.StockGrains.ContainsKey(Id) == false)
        {
            State.StockGrains.Add(Id, stockGrain);
        }

        if (State.BarcodeIndex.ContainsKey(stockInfo.Barcode) == false)
        {
            State.BarcodeIndex.Add(stockInfo.Barcode, stockInfo);
        }

        return Task.CompletedTask;
    }
}
