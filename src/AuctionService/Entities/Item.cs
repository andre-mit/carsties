using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionService.Entities;

[Table("Items")]
public class Item
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public string Make { get; set; }
    
    [MaxLength(100)]
    public string Model { get; set; }
    public int Year { get; set; }
    
    [MaxLength(50)]
    public string Color { get; set; }
    public int Mileage { get; set; }
    public string ImageUrl { get; set; }
    
    // Navigation properties
    public Guid AuctionId { get; set; }
    public Auction Auction { get; set; }
}