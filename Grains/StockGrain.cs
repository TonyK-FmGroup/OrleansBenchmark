using Orleans;

namespace Grains;

public interface IStockGrain : IGrainWithGuidKey
{
    Task SetInfo(StockInfo stockInfo);
    Task<StockInfo> GetStockInfo();
}

public class StockGrain : Grain, IStockGrain
{
    private readonly StockInfo State = new();

    public async Task SetInfo(StockInfo stockInfo)
    {
        var grainkey = this.GetPrimaryKeyString();
        var toStore = GrainFactory.GetGrain<IBranchStockGrain>($"{stockInfo.StoreId}");
        State.Title = stockInfo.Title;
        State.Quantity = stockInfo.Quantity;
        State.StoreId = stockInfo.StoreId;
        State.Barcode = stockInfo.Barcode;

        await toStore.ReceiveStock(grainkey, State, this.AsReference<IStockGrain>());
    }

    public Task<StockInfo> GetStockInfo()
    {
        return Task.FromResult(State);
    }
}
