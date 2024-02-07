using AutoMapper;
using Tegla.Application.Services.Items.Models;
using Tegla.Domain.Models.Items;

namespace Tegla.Application.Services.Items.Mappings;

public class ItemsMappingProfile : Profile
{
    public ItemsMappingProfile()
    {
        CreateMap<CreateItemRequest, Item>();
        CreateMap<Item, CreateItemResponse>();

        CreateMap<UpdateItemRequest, Item>();
        CreateMap<Item, UpdateItemResponse>();

        CreateMap<Item, RetriveItemByIdResponse>();
        CreateMap<Item, RemoveItemByIdResponse>();
    }
}
