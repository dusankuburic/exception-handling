using AutoMapper;
using Microsoft.Data.SqlClient;
using Tegla.Application.Services.Items.Models;
using Tegla.Domain.Models.Items;
using Tegla.Domain.Models.Items.Exceptions;
using Tegla.Persistence.Brokers.Loggings;
using Tegla.Persistence.Brokers.Storages;

namespace Tegla.Application.Services.Items;

public partial class ItemService : IItemService
{
    private readonly IStorageBroker _storageBroker;
    private readonly ILoggingBroker _loggingBroker;
    private readonly IMapper _mapper;

    public ItemService(
        IStorageBroker storageBroker,
        ILoggingBroker loggingBroker,
        IMapper mapper)
    {
        _storageBroker = storageBroker;
        _loggingBroker = loggingBroker;
        _mapper = mapper;
    }

    public ValueTask<CreateItemResponse> AddItem(CreateItemRequest item) =>
    TryCatch(async () =>
    {
        Item maybeItem = _mapper.Map<Item>(item);

        ValidateItemOnCreate(maybeItem);
        var res = await _storageBroker.InsertItem(maybeItem);

        return _mapper.Map<CreateItemResponse>(res);
    });

    public ValueTask<IEnumerable<Item>> ListAllItems() =>
    TryCatch(async () =>
    {
        var res = await _storageBroker.SelectAllItems();
        return res;
    });

    public ValueTask<UpdateItemResponse> ModifyItem(UpdateItemRequest item) =>
    TryCatch(async () =>
    {
        var maybeItem = _mapper.Map<Item>(item);

        ValidateItemOnUpdate(maybeItem);
        var res = await _storageBroker.UpdateItem(maybeItem);

        return _mapper.Map<UpdateItemResponse>(res);
    });

    public ValueTask<RetriveItemByIdResponse> RetriveItemById(Guid id) =>
    TryCatch(async () =>
    {
        var maybeItem = await _storageBroker.SelectItemById(id);
        ValidateItem(maybeItem);

        var res = _mapper.Map<RetriveItemByIdResponse>(maybeItem);
        return res;
    });

    public ValueTask<RemoveItemByIdResponse> RemoveItemById(Guid id) =>
    TryCatch(async () =>
    {
        var maybeItem = await _storageBroker.SelectItemById(id);
        ValidateItem(maybeItem);

        var deletedItem = await _storageBroker.DeleteItem(maybeItem);
        ValidateItem(maybeItem);

        var res = _mapper.Map<RemoveItemByIdResponse>(deletedItem);
        return res;
    });
}
