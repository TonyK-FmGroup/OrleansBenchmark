using Orleans;

namespace Grains;

[GenerateSerializer]
public class StockInfo
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public string Title { get; set; } = string.Empty;
    [Id(2)] public int Quantity { get; set; }
    [Id(3)] public string StoreId { get; set; } = string.Empty;
}
