using Orleans;

namespace Grains;

public interface IStockGrain : IGrainWithGuidKey
{
    Task SetInfo(StockInfo stockInfo);
    Task<StockInfo> GetStockInfo();
}

/// <summary>   
/// 
/// </summary>
public class StockGrain : Grain, IStockGrain
{
    // private StockInfo _stockInfo;
    private readonly StockInfo State = new();


    public async Task SetInfo(StockInfo stockInfo)
    {
        var grainkey = this.GetPrimaryKeyString();
        var toStore = GrainFactory.GetGrain<IBranchStockGrain>($"{stockInfo.StoreId}");
        State.Title = stockInfo.Title;
        State.Quantity = stockInfo.Quantity;
        State.StoreId = stockInfo.StoreId;

        await toStore.ReceiveStock(grainkey, State, this.AsReference<IStockGrain>());
        // await UpdateBranchStock();
    }

    public Task<StockInfo> GetStockInfo()
    {
        return Task.FromResult(State);
    }

    //private async Task UpdateBranchStock()
    //{
    //    var grainkey = this.GetPrimaryKeyString();
    //    var branchStock = GrainFactory.GetGrain<IBranchStockGrain>(State.StoreId);
    //    await branchStock.UpdateStockInfo(grainkey, State);
    //}
}
