using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[Route("api/auctions")]
[ApiController]
public class AuctionsController(ILogger<AuctionsController> logger, AuctionDbContext context, IMapper mapper)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions(string? date = null)
    {
        var query = context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

        if (!string.IsNullOrWhiteSpace(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        return await query.ProjectTo<AuctionDto>(mapper.ConfigurationProvider).ToListAsync();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(Guid id)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        return mapper.Map<AuctionDto>(auction);
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = mapper.Map<Auction>(createAuctionDto);
        // TODO: add current user as seller
        auction.Seller = "test";

        context.Auctions.Add(auction);
        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes to the database");

        return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        // TODO: check seller == username

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;

        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes to the database");

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await context.Auctions
            .FindAsync(id);

        if (auction == null) return NotFound();

        // TODO: check seller == username

        context.Auctions.Remove(auction);
        var result = await context.SaveChangesAsync() > 0;

        if (!result) return BadRequest("Could not save changes to the database");

        return NoContent();
    }
}