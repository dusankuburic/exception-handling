using Tegla.Domain.Models.Items;

namespace Tegla.Persistence.Brokers.Storages;

public partial interface IStorageBroker
{
    ValueTask<Item> InsertItem(Item item);
    ValueTask<List<Item>> SelectAllItems();
    ValueTask<Item> SelectItemById(Guid itemId);
    ValueTask<Item> UpdateItem(Item item);
    ValueTask<Item> DeleteItem(Item item);
}
