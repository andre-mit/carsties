using Microsoft.Extensions.Options;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Options;

namespace SearchService.Services;

public class AuctionSvcHttpClient(HttpClient httpClient, IOptions<AuctionSvcOptions> options)
{
    private readonly AuctionSvcOptions _options = options.Value;
    
    public async Task<List<Item>> GetItemsForSearchDb()
    {
        var lastUpdated = await DB.Find<Item, string>()
            .Sort(x => x.Descending(x => x.UpdatedAt))
            .Project(x => x.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        return await httpClient.GetFromJsonAsync<List<Item>>(_options.AuctionsUrl + $"?date={lastUpdated}");
    }
}