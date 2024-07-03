namespace SearchService.Options;

public class AuctionSvcOptions
{
    public const string AuctionService = "AuctionService";
    public string Url { get; set; }
    public string AuctionsEndpoint { get; set; }

    public string AuctionsUrl => $"{Url}/{AuctionsEndpoint}";
}