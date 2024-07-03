using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.RequestHelpers;

namespace SearchService.Controllers;

[Route("api/search")]
[ApiController]
public class SearchController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParams searchParams)
    {
        var query = DB.PagedSearch<Item, Item>();

        if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        
        query = searchParams.OrderBy switch
        {
            "make" => query.Sort(x => x.Ascending(a => a.Make)),
            "new" => query.Sort(x => x.Descending(a => a.CreatedAt)),
            "model" => query.Sort(x => x.Ascending(a => a.Model)),
            "year" => query.Sort(x => x.Ascending(a => a.Year)),
            "mileage" => query.Sort(x => x.Ascending(a => a.Mileage)),
            _ => query.Sort(x => x.Ascending(a => a.AuctionEnd))
        };
        
        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6) && x.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow)
        };
        
        if(!string.IsNullOrWhiteSpace(searchParams.Seller))
            query.Match(x => x.Seller == searchParams.Seller);
        
        if(!string.IsNullOrWhiteSpace(searchParams.Winner))
            query.Match(x => x.Winner == searchParams.Winner);

        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        var results = await query.ExecuteAsync();

        return Ok(new
        {
            results = results.Results,
            total = results.TotalCount,
            pages = results.PageCount
        });
    }
}