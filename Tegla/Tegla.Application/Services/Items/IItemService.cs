using Tegla.Application.Services.Items.Models;
using Tegla.Domain.Models.Items;

namespace Tegla.Application.Services.Items;

public interface IItemService
{
    ValueTask<CreateItemResponse> AddItem(CreateItemRequest item);
    ValueTask<IEnumerable<Item>> ListAllItems();
    ValueTask<UpdateItemResponse> ModifyItem(UpdateItemRequest item);
    ValueTask<RetriveItemByIdResponse> RetriveItemById(Guid id);
    ValueTask<RemoveItemByIdResponse> RemoveItemById(Guid id);
}
