using Orleans;

namespace Grains;

public interface IBranchStockGrain : IGrainWithStringKey
{
    Task ReceiveStock(string Id, StockInfo stockItem, IStockGrain stockGrain);

    // Task RemoveStock(string Id);

    // Task UpdateStockInfo(string Id, StockInfo stockItem);

    Task<Dictionary<string, StockInfo>> GetStock();

    Task<Dictionary<string, StockInfo>> GetStockFromGrains();

}

/// <summary>
/// Maintains a collection of stock that belongs to a given store
/// The grain Id is the store ID.
/// </summary>
public class BranchStockGrain : IBranchStockGrain
{
    private readonly BranchStockInfo State = new();


    public Task<Dictionary<string, StockInfo>> GetStock()
    {
        return Task.FromResult(State.Stock);
    }

    public Task<Dictionary<string, StockInfo>> GetStockFromGrains()
    {
        var info = new Dictionary<string, StockInfo>();

        foreach (var item in State.StockGrains)
        {
            info.Add(item.Key, item.Value.GetStockInfo().Result);
        }

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

        return Task.CompletedTask;
    }

    //public Task RemoveStock(string Id)
    //{
    //    State.Stock.Remove(Id);
    //    return Task.CompletedTask;
    //}

    //public Task UpdateStockInfo(string Id, StockInfo stockItem)
    //{
    //    if (State.Stock.ContainsKey(Id))
    //    {
    //        State.Stock[Id] = stockItem;
    //    }
    //    return Task.CompletedTask;
    //}
}
