using Microsoft.EntityFrameworkCore;
using Tegla.Domain.Models.Items;

namespace Tegla.Persistence.Brokers.Storages;
public partial class StorageBroker
{
    public async ValueTask<Item> DeleteItem(Item item) {
        _ctx.Items.Remove(item);
        await _ctx.SaveChangesAsync();

        return item;
    }

    public async ValueTask<Item> InsertItem(Item item) {
        await _ctx.Items.AddAsync(item);
        await _ctx.SaveChangesAsync();

        return item;
    }

    public async ValueTask<List<Item>> SelectAllItems() {
        return await _ctx.Items.ToListAsync();
    }

    public async ValueTask<Item> SelectItemById(Guid itemId) {
        return await _ctx.Items.FirstOrDefaultAsync(x => x.Id == itemId);
    }

    public async ValueTask<Item> UpdateItem(Item item) {
        _ctx.Items.Update(item);
        await _ctx.SaveChangesAsync();

        return item;
    }
}
