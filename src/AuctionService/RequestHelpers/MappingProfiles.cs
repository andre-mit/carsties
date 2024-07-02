using AutoMapper;
using AuctionService.DTOs;
using AuctionService.Entities;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(destination => destination.Item,
                options => options.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();
    }
}